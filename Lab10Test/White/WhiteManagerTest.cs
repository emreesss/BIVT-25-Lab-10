//using Lab10.White;
//using System.Text.Json;

//namespace Lab10Test.White
//{
//    [TestClass]
//    public sealed class WhiteManagerTest
//    {
//        private Lab9.White.White[] _tasks;
//        private string[] _input;
//        private string[,] _codes;

//        [TestInitialize]
//        public void LoadData()
//        {
//            var folder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
//            var file = Path.Combine(folder, "Lab10Test", "White", "data.json");

//            var json = JsonSerializer.Deserialize<JsonElement>(File.ReadAllText(file));

//            _input = json.GetProperty("Task3").GetProperty("input").Deserialize<string[]>();

//            var codesArray = json.GetProperty("Task3").GetProperty("codes").Deserialize<string[][]>();
//            _codes = new string[codesArray.Length, 2];

//            for (int i = 0; i < codesArray.Length; i++)
//            {
//                _codes[i, 0] = codesArray[i][0];
//                _codes[i, 1] = codesArray[i][1];
//            }
//        }

//        private void Init(int i)
//        {
//            _tasks = new Lab9.White.White[]
//            {
//                 new Lab9.White.Task1(_input[i]),
//                 new Lab9.White.Task2(_input[i]),
//                 new Lab9.White.Task3(_input[i], _codes),
//                 new Lab9.White.Task4(_input[i])
//            };

//            foreach (var t in _tasks)
//                t.Review();
//        }

//        [TestMethod]
//        public void Test_00_OOP()
//        {
//            var type = typeof(Lab10.White.White);

//            Assert.IsTrue(type.IsClass, "White must be class");

//            Assert.IsNotNull(type.GetProperty("Manager"));
//            Assert.IsNotNull(type.GetProperty("Tasks"));

//            Assert.IsNotNull(type.GetMethod("Add", new[] { typeof(Lab9.White.White) }));
//            Assert.IsNotNull(type.GetMethod("Add", new[] { typeof(Lab9.White.White[]) }));
//            Assert.IsNotNull(type.GetMethod("Remove"));
//            Assert.IsNotNull(type.GetMethod("Clear"));
//            Assert.IsNotNull(type.GetMethod("SaveTasks"));
//            Assert.IsNotNull(type.GetMethod("LoadTasks"));
//            Assert.IsNotNull(type.GetMethod("ChangeManager"));
//        }

//        [TestMethod]
//        public void Test_01_Add_Remove_AllTasks()
//        {
//            for (int i = 0; i < _input.Length; i++)
//            {
//                Init(i);

//                var w = new Lab10.White.White();

//                w.Add(_tasks);
//                Assert.AreEqual(_tasks.Length, w.Tasks.Length, $"Add failed test {i}");

//                w.Remove(_tasks[0]);
//                Assert.IsFalse(w.Tasks.Contains(_tasks[0]), $"Remove failed test {i}");
//            }
//        }

//        [TestMethod]
//        public void Test_02_Save_Load_Txt()
//        {
//            for (int i = 0; i < _input.Length; i++)
//            {
//                Init(i);

//                var manager = new WhiteTxtFileManager("txt");
//                var folder = Path.Combine(Path.GetTempPath(), $"WhiteTxt_{i}");
//                Directory.CreateDirectory(folder);

//                manager.SelectFolder(folder);

//                var w = new Lab10.White.White(manager, _tasks);

//                w.SaveTasks();
//                w.LoadTasks();

//                for (int j = 0; j < _tasks.Length; j++)
//                {
//                    Assert.AreEqual(_tasks[j].Input, w.Tasks[j].Input,
//                        $"TXT load mismatch test {i} task {j}");
//                }

//                Directory.Delete(folder, true);
//            }
//        }

//        [TestMethod]
//        public void Test_03_Save_Load_Json()
//        {
//            for (int i = 0; i < _input.Length; i++)
//            {
//                Init(i);

//                var manager = new WhiteJsonFileManager("json");
//                var folder = Path.Combine(Path.GetTempPath(), $"WhiteJson_{i}");
//                Directory.CreateDirectory(folder);

//                manager.SelectFolder(folder);

//                var w = new Lab10.White.White(manager, _tasks);

//                w.SaveTasks();
//                w.LoadTasks();

//                for (int j = 0; j < _tasks.Length; j++)
//                {
//                    Assert.AreEqual(_tasks[j].Input, w.Tasks[j].Input,
//                        $"JSON load mismatch test {i} task {j}");
//                }

//                Directory.Delete(folder, true);
//            }
//        }

//        [TestMethod]
//        public void Test_04_ChangeManager_And_Format()
//        {
//            for (int i = 0; i < _input.Length; i++)
//            {
//                Init(i);

//                var txtManager = new WhiteTxtFileManager("txt");
//                var jsonManager = new WhiteJsonFileManager("json");

//                var folder = Path.Combine(Path.GetTempPath(), $"WhiteMix_{i}");
//                Directory.CreateDirectory(folder);

//                txtManager.SelectFolder(folder);

//                var w = new Lab10.White.White(txtManager, _tasks);

//                w.SaveTasks();

//                w.ChangeManager(jsonManager);
//                w.LoadTasks();

//                for (int j = 0; j < w.Tasks.Length; j++)
//                {
//                    Assert.IsTrue(w.Tasks[j] == null || w.Tasks[j].Input != _tasks[j].Input,
//                        $"Format switch should break deserialize test {i} task {j}");
//                }

//                var newFolder = Path.Combine(Directory.GetCurrentDirectory(), "json");
//                if (Directory.Exists(newFolder))
//                    Directory.Delete(newFolder, true);

//                Directory.Delete(folder, true);
//            }
//        }

//        [TestMethod]
//        public void Test_05_Clear_All()
//        {
//            for (int i = 0; i < _input.Length; i++)
//            {
//                Init(i);

//                var manager = new WhiteTxtFileManager("clear");
//                var folder = Path.Combine(Path.GetTempPath(), $"WhiteClear_{i}");
//                Directory.CreateDirectory(folder);

//                manager.SelectFolder(folder);

//                var w = new Lab10.White.White(manager, _tasks);

//                w.Clear();

//                Assert.AreEqual(0, w.Tasks.Length, $"Clear failed test {i}");
//                Assert.IsFalse(Directory.Exists(folder), $"Folder not deleted test {i}");
//            }
//        }
//    }
//}