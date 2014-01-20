
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace MapFarce.DataModel
{
    public class Project
    {
        public static Project Instance { get; set; }

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

        public void SaveTo(FileInfo fi)
        {
            filename = fi;

            hasChanges = false;
            foreach (var source in sources)
                source.HasChanges = false;
            foreach (var mapping in mappings)
                mapping.HasChanges = false;
        }

        public static Project LoadFrom(FileInfo fi)
        {
            Project p = new Project();
            p.filename = fi;

            return p;
        }

        public static bool Open()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All Files (*.*)|*.*";

            if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return false;

            var p = Project.LoadFrom(new FileInfo(ofd.FileName));
            if (p != null)
            {
                Instance = p;
                return true;
            }
            return false;
        }
    }
}
