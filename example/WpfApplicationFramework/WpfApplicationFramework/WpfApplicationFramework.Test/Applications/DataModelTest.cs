using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Waf.Applications;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Waf.Applications
{
    [TestClass]
    public class DataModelTest
    {
        [TestMethod]
        public void SerializationTest()
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream stream = new MemoryStream())
            {
                PersonDataModel person = new PersonDataModel() { Name = "Hugo" };
                formatter.Serialize(stream, person);

                stream.Position = 0;
                PersonDataModel newPerson = (PersonDataModel)formatter.Deserialize(stream);
                Assert.AreEqual(person.Name, newPerson.Name);
            }
        }



        [Serializable]
        private class PersonDataModel : DataModel
        {
            private string name;

            public string Name
            {
                get { return name; }
                set
                {
                    if (name != value)
                    {
                        name = value;
                        RaisePropertyChanged("Name");
                    }
                }
            }
        }
    }
}
