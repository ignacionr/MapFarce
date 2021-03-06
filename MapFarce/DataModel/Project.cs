﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using MapFarce.UI;

namespace MapFarce.DataModel
{
    public class Project
    {
        public static Project Instance { get; set; }
        public static ProjectPanel Panel { get; set; }

        List<DataSource> sources = new List<DataSource>();
        List<Mapping> mappings = new List<Mapping>();

        public IList<DataSource> Sources { get { return sources.AsReadOnly(); } }
        public IList<Mapping> Mappings { get { return mappings.AsReadOnly(); } }

        private bool hasChanges = false;

        public void AddSource(DataSource source)
        {
            sources.Add(source);
            hasChanges = true;
        }

        public void RemoveSource(DataSource source)
        {
            sources.Remove(source);
            hasChanges = true;
        }

        public void AddMapping(Mapping mapping)
        {
            mappings.Add(mapping);
            hasChanges = true;
        }

        public void RemoveMapping(Mapping mapping)
        {
            mappings.Remove(mapping);
            hasChanges = true;
        }

        public bool HasChanges
        {
            get
            {
                if (hasChanges)
                    return true;

                foreach (DataSource source in sources)
                    if (source.HasChanges)
                        return true;
                foreach (Mapping mapping in mappings)
                    if (mapping.HasChanges)
                        return true;

                return false;
            }
        }

        public void Changed()
        {
            hasChanges = true;
        }

        private FileInfo ShowSaveDialog()
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "Project Files|*.farce|All Files (*.*)|*.*";

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return null;

            return new FileInfo(dialog.FileName);
        }

        FileInfo filename;
        public bool Save()
        {
            FileInfo filename = this.filename;
            if (filename == null)
            {
                filename = ShowSaveDialog();
                if (filename == null)
                    return false;
            }

            SaveTo(filename);
            return true;
        }

        public bool SaveAs()
        {
            FileInfo filename = ShowSaveDialog();
            if (filename == null)
                return false;

            SaveTo(filename);
            return true;
        }

        private const string projectNodeName = "Project", sourcesNodeName = "Sources", mappingsNodeName = "Mappings";
        public void SaveTo(FileInfo fi)
        {
            filename = fi;

            XmlDocument doc = new XmlDocument();
            XmlNode root = doc.CreateElement(projectNodeName);
            doc.AppendChild(root);

            XmlNode sourceRoot = doc.CreateElement(sourcesNodeName);
            root.AppendChild(sourceRoot);

            foreach (var source in sources)
            {
                var sourceNode = source.CreateXmlNode(sourceRoot);
                
                var typesRoot = doc.CreateElement("DataTypes");
                sourceNode.AppendChild(typesRoot);

                foreach (var type in source)
                {
                    var typeNode = type.CreateXmlNode(typesRoot);
                }

                source.HasChanges = false;
            }

            XmlNode mappingRoot = doc.CreateElement(mappingsNodeName);
            root.AppendChild(mappingRoot);

            foreach (var mapping in mappings)
            {
                var node = mapping.CreateXmlNode(mappingRoot);
                mapping.HasChanges = false;
            }

            doc.Save(filename.FullName);
            hasChanges = false;
        }

        public static Project LoadFrom(FileInfo fi)
        {
            Project p = Instance = new Project();
            p.filename = fi;

            try
            {

                XmlDocument doc = new XmlDocument();
                doc.Load(fi.FullName);

                if (doc.DocumentElement.Name != projectNodeName || doc.DocumentElement.ChildNodes.Count < 2)
                    return null;

                var sourceRoot = doc.DocumentElement.ChildNodes[0];
                var mappingsRoot = doc.DocumentElement.ChildNodes[1];

                if (sourceRoot.Name != sourcesNodeName || mappingsRoot.Name != mappingsNodeName)
                    return null;

                foreach (XmlNode node in sourceRoot.ChildNodes)
                {
                    DataSource source = DataSource.LoadFromXml(node);
                    p.sources.Add(source);

                    Panel.AddControlFor(source);
                    source.LoadBounds(node);
                }

                foreach (XmlNode node in mappingsRoot.ChildNodes)
                {
                    Mapping mapping = Mapping.LoadFromXml(node);
                    p.mappings.Add(mapping);
                    Panel.AddControlFor(mapping);
                    mapping.LoadBounds(node);
                }
            }
            catch (FormatException ex)
            {
                return null;
            }
            catch (KeyNotFoundException ex)
            {
                return null;
            }

            return p;
        }

        public static bool Open(ProjectPanel panel)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All Files (*.*)|*.*";

            if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return false;

            panel.Reset();

            var p = Project.LoadFrom(new FileInfo(ofd.FileName));
            if (p == null)
            {
                Instance = null;
                return false;
            }

            return true;
        }

        public void RunMappings()
        {
            foreach ( var source in sources )
                if (source.DataMode == DataSource.Mode.Output)
                {
                    source.BeginWrite();

                    foreach (var type in source)
                    {
                        type.BeginWrite();

                        foreach (var connection in type.Connections)
                        {
                            Mapping m = connection.Mapping;

                            foreach (var input in m.Inputs)
                                m.Perform(input, connection);
                        }

                        type.FinishWrite();
                    }

                    source.FinishWrite();
                }
        }
    }
}
