using System;
using System.IO;

namespace Lab10.White
{
    public class White
    {
        private Lab9.White.White[] _tasks;
        private WhiteFileManager _manager;

        public WhiteFileManager Manager => _manager;
        public Lab9.White.White[] Tasks => _tasks;

        public White(Lab9.White.White[] tasks = null)
        {
            _tasks = CopyTasks(tasks);
            _manager = null;
        }

        public White(WhiteFileManager manager, Lab9.White.White[] tasks = null)
        {
            _manager = manager;
            _tasks = CopyTasks(tasks);
        }

        public White(Lab9.White.White[] tasks, WhiteFileManager manager)
        {
            _manager = manager;
            _tasks = CopyTasks(tasks);
        }

        private static Lab9.White.White[] CopyTasks(Lab9.White.White[] tasks)
        {
            if (tasks == null)
                return new Lab9.White.White[0];

            var copy = new Lab9.White.White[tasks.Length];
            Array.Copy(tasks, copy, tasks.Length);
            return copy;
        }

        public void Add(Lab9.White.White task)
        {
            if (task == null) return;

            var newArr = new Lab9.White.White[_tasks.Length + 1];
            Array.Copy(_tasks, newArr, _tasks.Length);
            newArr[_tasks.Length] = task;
            _tasks = newArr;
        }

        public void Add(Lab9.White.White[] tasks)
        {
            if (tasks == null) return;
            foreach (var t in tasks)
                Add(t);
        }

        public void Remove(Lab9.White.White task)
        {
            if (task == null || _tasks.Length == 0) return;

            int idx = Array.IndexOf(_tasks, task);
            if (idx < 0) return;

            var newArr = new Lab9.White.White[_tasks.Length - 1];
            for (int i = 0, j = 0; i < _tasks.Length; i++)
            {
                if (i == idx) continue;
                newArr[j++] = _tasks[i];
            }
            _tasks = newArr;
        }

        public void Clear()
        {
            _tasks = new Lab9.White.White[0];

            if (_manager != null && !string.IsNullOrEmpty(_manager.FolderPath) && Directory.Exists(_manager.FolderPath))
            {
                try { Directory.Delete(_manager.FolderPath, true); }
                catch { }
            }
        }

        public void SaveTasks()
        {
            if (_manager == null) return;

            for (int i = 0; i < _tasks.Length; i++)
            {
                if (_tasks[i] == null) continue;
                _manager.ChangeFileName("task" + i);
                _manager.Serialize(_tasks[i]);
            }
        }

        public void LoadTasks()
        {
            if (_manager == null) return;

            for (int i = 0; i < _tasks.Length; i++)
            {
                _manager.ChangeFileName("task" + i);
                var loaded = _manager.Deserialize();
                _tasks[i] = loaded;
            }
        }

        public void ChangeManager(WhiteFileManager newManager)
        {
            if (newManager == null) return;

            _manager = newManager;

            string folder = newManager.Name;
            if (string.IsNullOrEmpty(folder))
                folder = "manager";

            string full = Path.Combine(Directory.GetCurrentDirectory(), folder);
            if (!Directory.Exists(full))
                Directory.CreateDirectory(full);

            newManager.SelectFolder(full);
        }
    }
}
