using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Q41.DocumentService.TemplateManager.Models;


namespace Q41.DocumentService.TemplateManager.Util
{
    public static class SerializationUtil 
    {
        private static readonly log4net.ILog log =
           log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string Serialize<T>(T data) where T : new()
        {

            //string podaci = data.ToString();
            //podaci = ReplaceHexadecimalSymbols(podaci);
            //data = podaci.ToDictionary<T>(T data);

            if (data is Dictionary<string, string>)
            {
                var dict = data as Dictionary<string, string>;
                var lines = dict.Select(kvp => kvp.Key + ": " + kvp.Value.ToString());

                log.DebugFormat("Serijalizacija: {0}", string.Join(Environment.NewLine, lines));
            }

            var xmlWritterSettings = new XmlWriterSettings() { CheckCharacters = false };
            xmlWritterSettings.Indent = true;

            // remove:character (0x001A) ...
            // T-SQL: replace(description,CHAR(25),'')
            //data = data; 

            var serializer = new DataContractSerializer(typeof(SerializationWrapper<T>));

            using (var sw = new StringWriter())
            {

                using (var writer = XmlWriter.Create(sw, xmlWritterSettings))
                {
                    serializer.WriteObject(writer, new SerializationWrapper<T> { Value = data });
                    writer.Flush();
                    return sw.ToString();
                }

                ////  ako je lista:
                //try
                //{
                //    using (Stream stream = File.Open("data.bin", FileMode.Create))
                //    {
                //        BinaryFormatter bin = new BinaryFormatter();
                //        bin.Serialize(stream, lizards1);
                //    }
                //}
                //catch (IOException)
                //{
                //}


            }
        }

        public static T Deserialize<T>(string xml) where T : new()
        {
            //xml = Regex.Replace(xml, @"\s+xsi:type=""\w+""", "");
            xml = xml.Replace("http://schemas.datacontract.org/2004/07/ApisIt.DocumentService.Dal.Util", "http://schemas.datacontract.org/2004/07/Q41.DocumentService.TemplateManager.Util");

            xml = xml.Replace("SerializationUtil.SerializationWrapperOfArrayOfKeyValueOfstringProcedureResultFieldzS_SZ5M7cdWeQgFjH", "SerializationUtil.SerializationWrapperOfArrayOfKeyValueOfstringProcedureResultFieldFRfG1zYydWeQgFjH");

            xml = xml.Replace("zS_SZ5M7c", "FRfG1zYy");

            xml = xml.Replace("ApisIt.DocumentService.Model", "Q41.DocumentService.TemplateManager.Models");

            var xmlReaderSettings = new XmlReaderSettings() { CheckCharacters = false };

            var serializer = new DataContractSerializer(typeof(SerializationWrapper<T>));
            //using (var sr = new StringReader(xml))
            using (TextReader sr = new StringReader(xml))
            {
                //var reader = XmlReader.Create(sr, xmlReaderSettings);

                using (XmlTextReader reader = new XmlTextReader(sr))
                {
                    //reader.Namespaces = false;

                    //reader.Settings.CheckCharacters = false;

                    SerializationWrapper<T> data = new SerializationWrapper<T>();
                    try
                    {
                        data = (SerializationWrapper<T>)serializer.ReadObject(reader);
                        return data.Value;
                    }
                    catch (Exception ex)
                    {
                        //
                        log.ErrorFormat("Nastala je greška prilikom Deserialize. \n{0}", ex.ToString());
                        return data.Value;
                    }
                }
            }

        }

        public static T DeserializeFull<T>(string xml) where T : new()
        {
            //xml = Regex.Replace(xml, @"\s+xsi:type=""\w+""", "");

            xml = xml.Replace("http://schemas.datacontract.org/2004/07/ApisIt.DocumentService.Model","http://schemas.datacontract.org/2004/07/Q41.DocumentService.TemplateManager.Models" );
            xml = xml.Replace("http://schemas.datacontract.org/2004/07/ApisIt.DocumentService.Dal.Util","http://schemas.datacontract.org/2004/07/Q41.DocumentService.TemplateManager.Util");
            xml = xml.Replace("ApisIt.DocumentService.Model", "Q41.DocumentService.TemplateManager.Models");           
            //... "zS_SZ5M7c" ... -> ... "FRfG1zYy" ...
            xml = xml.Replace("SerializationUtil.SerializationWrapperOfArrayOfKeyValueOfstringProcedureResultFieldzS_SZ5M7cdWeQgFjH", "SerializationUtil.SerializationWrapperOfArrayOfKeyValueOfstringProcedureResultFieldFRfG1zYydWeQgFjH");
            xml = xml.Replace("zS_SZ5M7c","FRfG1zYy");
            xml = xml.Replace("KeyValueOfstringTemplateFieldzS_SZ5M7c","KeyValueOfstringTemplateFieldFRfG1zYy");

            var xmlReaderSettings = new XmlReaderSettings() { CheckCharacters = false };

            var serializer = new DataContractSerializer(typeof(SerializationWrapper<T>));
            //using (var sr = new StringReader(xml))
            using (TextReader sr = new StringReader(xml))
            {
                //var reader = XmlReader.Create(sr, xmlReaderSettings);

                using (XmlTextReader reader = new XmlTextReader(sr))
                {
                    //reader.Namespaces = false;

                    //reader.Settings.CheckCharacters = false;

                    SerializationWrapper<T> data = new SerializationWrapper<T>();
                    try
                    {
                        data = (SerializationWrapper<T>)serializer.ReadObject(reader);
                        return data.Value;
                    }
                    catch (Exception ex)
                    {
                        //
                        log.ErrorFormat("Nastala je greška prilikom Deserialize. \n{0}", ex.ToString());
                        return data.Value;
                    }
                }
            }

        }

        public static T DeserializeComplex<T>(string xml) where T : new()
        {
            //xml = Regex.Replace(xml, @"\s+xsi:type=""\w+""", "");
            xml = xml.Replace("http://schemas.datacontract.org/2004/07/ApisIt.DocumentService.Dal.Util", "http://schemas.datacontract.org/2004/07/Q41.DocumentService.TemplateManager.Util");
                                                                                                             
            xml = xml.Replace("SerializationUtil.SerializationWrapperOfArrayOfKeyValueOfstringProcedureResultFieldzS_SZ5M7cdWeQgFjH","SerializationUtil.SerializationWrapperOfArrayOfKeyValueOfstringProcedureResultFieldFRfG1zYydWeQgFjH");

            xml = xml.Replace("zS_SZ5M7c", "FRfG1zYy");

            xml = xml.Replace("ApisIt.DocumentService.Model", "Q41.DocumentService.TemplateManager.Models");

            // novo 
            xml = xml.Replace("SerializationUtil.SerializationWrapperOfArrayOfKeyValueOfstringProcedureResultFieldFRfG1zYydWeQgFjH", "SerializationUtil.SerializationWrapperOfArrayOfKeyValueOfstringProcedureComplexResultFieldFRfG1zYydWeQgFjH");



            var xmlReaderSettings = new XmlReaderSettings() { CheckCharacters = false };

            var serializer = new DataContractSerializer(typeof(SerializationWrapper<T>));
            //using (var sr = new StringReader(xml))
            using (TextReader sr = new StringReader(xml))
            {
                using (XmlTextReader reader = new XmlTextReader(sr))
                {

                    do
                    {
                        if (reader.Name == "Member" && reader.IsStartElement())
                        {
                            Type type = System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                                              .Where(x => x.Name == reader.Name)
                                              .FirstOrDefault();
                            if (type != null)
                            {
                                var xmlSerializer = new XmlSerializer(type);
                                //var member = (IMember)xmlSerializer.Deserialize(reader);
                                //this.Add(member);
                            }
                            continue; 

                        }

                        if (reader.Name == "Fields")
                        {
                            var miki = ""; 

                        }
                        if (reader.Name == "FamilyTree" && reader.NodeType == XmlNodeType.EndElement)
                            break;

                        reader.Read();
                    } while (!reader.EOF);

                    SerializationWrapper<T> data = new SerializationWrapper<T>();
                    try
                    {
                        data = (SerializationWrapper<T>)serializer.ReadObject(reader);
                        return data.Value;
                    }
                    catch(Exception ex)
                    {
                        //
                        log.ErrorFormat("Nastala je greška prilikom DeserializeComplex. \n{0}", ex.ToString());
                        return data.Value;
                    }





                }
            }

        }
        public static T DeserializeWithConvert<T>(string xml) where T : new()
        {
            xml = xml.Replace("SerializationUtil.SerializationWrapperOfArrayOfTemplateFieldBinding8kKS2llj", "SerializationUtil.SerializationWrapperOfArrayOfTemplateFieldBindingMeydyfss");
            xml = xml.Replace("http://schemas.datacontract.org/2004/07/ApisIt.DocumentService.Dal.Util", "http://schemas.datacontract.org/2004/07/Q41.DocumentService.TemplateManager.Util");
            xml = xml.Replace("http://schemas.datacontract.org/2004/07/ApisIt.DocumentService.Model", "http://schemas.datacontract.org/2004/07/Q41.DocumentService.TemplateManager.Models");



            var xmlReaderSettings = new XmlReaderSettings() { CheckCharacters = false };

            var serializer = new DataContractSerializer(typeof(SerializationWrapper<T>));
            using (TextReader sr = new StringReader(xml))
            {
                using (XmlTextReader reader = new XmlTextReader(sr))
                {
                    SerializationWrapper<T> data = new SerializationWrapper<T>();
                    try
                    {
                        data = (SerializationWrapper<T>)serializer.ReadObject(reader);
                        return data.Value;
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat("Nastala je greška prilikom DeserializeWithConvert. \n{0}", ex.ToString());
                        return data.Value;
                    }
                }
            }

        }
        public static T DeserializeTemplateParameter<T>(string xml) where T : new()
        {

            xml = xml.Replace("http://schemas.datacontract.org/2004/07/ApisIt.DocumentService.Dal.Util", "http://schemas.datacontract.org/2004/07/Q41.DocumentService.TemplateManager.Util");
            xml = xml.Replace("http://schemas.datacontract.org/2004/07/ApisIt.DocumentService.Model", "http://schemas.datacontract.org/2004/07/Q41.DocumentService.TemplateManager.Models");
            xml = xml.Replace("SerializationUtil.SerializationWrapperOfArrayOfTemplateParameterMeydyfss", "SerializationUtil.SerializationWrapperOfArrayOfKeyValueOfstringTemplateParameterFRfG1zYydWeQgFjH");
            xml = xml.Replace("SerializationUtil.SerializationWrapperOfArrayOfTemplateParameter8kKS2llj", "SerializationUtil.SerializationWrapperOfArrayOfKeyValueOfstringTemplateParameterFRfG1zYydWeQgFjH");


            var xmlReaderSettings = new XmlReaderSettings() { CheckCharacters = false };

            var serializer = new DataContractSerializer(typeof(SerializationWrapper<T>));
            using (TextReader sr = new StringReader(xml))
            {
                using (XmlTextReader reader = new XmlTextReader(sr))
                {
                    SerializationWrapper<T> data = new SerializationWrapper<T>();
                    try
                    {
                        data = (SerializationWrapper<T>)serializer.ReadObject(reader);
                        return data.Value;
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat("Nastala je greška prilikom DeserializeTemplateParameter. \n{0}", ex.ToString());
                        return data.Value;
                    }
                }
            }

        }
        public static T DeserializeTemplateParameter2<T>(string xml) where T : new()
        {

            xml = xml.Replace("http://schemas.datacontract.org/2004/07/ApisIt.DocumentService.Dal.Util", "http://schemas.datacontract.org/2004/07/Q41.DocumentService.TemplateManager.Util");
            xml = xml.Replace("http://schemas.datacontract.org/2004/07/ApisIt.DocumentService.Model", "http://schemas.datacontract.org/2004/07/Q41.DocumentService.TemplateManager.Models");
            xml = xml.Replace("SerializationUtil.SerializationWrapperOfArrayOfKeyValueOfstringTemplateFieldzS_SZ5M7cdWeQgFjH", "SerializationUtil.SerializationWrapperOfArrayOfTemplateParameterMeydyfss");
            xml = xml.Replace("KeyValueOfstringTemplateField", "TemplateParameter");
            xml = xml.Replace("zS_SZ5M7cdWeQgFjH", "Meydyfss");
            xml = xml.Replace("zS_SZ5M7c", "FRfG1zYy");
            var xmlReaderSettings = new XmlReaderSettings() { CheckCharacters = false };

            var serializer = new DataContractSerializer(typeof(SerializationWrapper<T>));
            using (TextReader sr = new StringReader(xml))
            {
                using (XmlTextReader reader = new XmlTextReader(sr))
                {
                    SerializationWrapper<T> data = new SerializationWrapper<T>();
                    try
                    {
                        data = (SerializationWrapper<T>)serializer.ReadObject(reader);
                        return data.Value;
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat("Nastala je greška prilikom DeserializeTemplateParameter2. \n{0}", ex.ToString());
                        return data.Value;
                    }
                }
            }

        }
        public static T DeserializeTemplateFields<T>(string xml) where T : new()
        {

            xml = xml.Replace("http://schemas.datacontract.org/2004/07/ApisIt.DocumentService.Dal.Util", "http://schemas.datacontract.org/2004/07/Q41.DocumentService.TemplateManager.Util");
            xml = xml.Replace("http://schemas.datacontract.org/2004/07/ApisIt.DocumentService.Model", "http://schemas.datacontract.org/2004/07/Q41.DocumentService.TemplateManager.Models");
            //xml = xml.Replace("SerializationUtil.SerializationWrapperOfArrayOfKeyValueOfstringTemplateFieldzS_SZ5M7cdWeQgFjH", "SerializationUtil.SerializationWrapperOfArrayOfTemplateParameterMeydyfss");
            //xml = xml.Replace("KeyValueOfstringTemplateField", "TemplateParameter");
            //xml = xml.Replace("zS_SZ5M7cdWeQgFjH", "Meydyfss");
            //xml = xml.Replace("zS_SZ5M7c", "FRfG1zYy");
            //xml = xml.Replace("zS_SZ5M7cdWeQgFjH", "FRfG1zYydWeQgFjH");

            xml = xml.Replace("SerializationUtil.SerializationWrapperOfArrayOfKeyValueOfstringTemplateFieldzS_SZ5M7cdWeQgFjH", "SerializationUtil.SerializationWrapperOfArrayOfKeyValueOfstringTemplateFieldFRfG1zYydWeQgFjH");

            var xmlReaderSettings = new XmlReaderSettings() { CheckCharacters = false };

            var serializer = new DataContractSerializer(typeof(SerializationWrapper<T>));
            using (TextReader sr = new StringReader(xml))
            {
                using (XmlTextReader reader = new XmlTextReader(sr))
                {
                    SerializationWrapper<T> data = new SerializationWrapper<T>();
                    try
                    {
                        data = (SerializationWrapper<T>)serializer.ReadObject(reader);
                        return data.Value;
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat("Nastala je greška prilikom DeserializeTemplateFields. \n{0}", ex.ToString());
                        return data.Value;
                    }
                }
            }

        }
        public static T DeserializeStoreParameter<T>(string xml) where T : new()
        {
            //xml = xml.Replace("SerializationUtil.SerializationWrapperOfArrayOfTemplateFieldBinding8kKS2llj", "SerializationUtil.SerializationWrapperOfArrayOfTemplateFieldBindingMeydyfss");
            xml = xml.Replace("http://schemas.datacontract.org/2004/07/ApisIt.DocumentService.Dal.Util", "http://schemas.datacontract.org/2004/07/Q41.DocumentService.TemplateManager.Util");
            //xml = xml.Replace("http://schemas.datacontract.org/2004/07/ApisIt.DocumentService.Model", "http://schemas.datacontract.org/2004/07/Q41.DocumentService.TemplateManager.Models");

            xml = xml.Replace("ProcedureResultFieldMeydyfss", "KeyValueOfstringProcedureResultFieldzS_SZ5M7cdWeQgFjH");
            //xml = xml.Replace("", "");

            var xmlReaderSettings = new XmlReaderSettings() { CheckCharacters = false };

            var serializer = new DataContractSerializer(typeof(SerializationWrapper<T>));
            using (TextReader sr = new StringReader(xml))
            {
                using (XmlTextReader reader = new XmlTextReader(sr))
                {
                    SerializationWrapper<T> data = new SerializationWrapper<T>();
                    try
                    {
                        data = (SerializationWrapper<T>)serializer.ReadObject(reader);
                        return data.Value;
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat("Nastala je greška prilikom DeserializeStoreParameter. \n{0}", ex.ToString());
                        return data.Value;
                    }
                }
            }

        }

        public static T DeserializeTemplateParameter3<T>(string xml) where T : new()
        {

            xml = xml.Replace("http://schemas.datacontract.org/2004/07/ApisIt.DocumentService.Dal.Util", "http://schemas.datacontract.org/2004/07/Q41.DocumentService.TemplateManager.Util");
            xml = xml.Replace("http://schemas.datacontract.org/2004/07/ApisIt.DocumentService.Model", "http://schemas.datacontract.org/2004/07/Q41.DocumentService.TemplateManager.Models");
            xml = xml.Replace("SerializationUtil.SerializationWrapperOfArrayOfKeyValueOfstringTemplateFieldzS_SZ5M7cdWeQgFjH", "SerializationUtil.SerializationWrapperOfArrayOfTemplateParameterMeydyfss");
            xml = xml.Replace("KeyValueOfstringTemplateField", "TemplateParameter");
            xml = xml.Replace("zS_SZ5M7cdWeQgFjH", "Meydyfss");
            xml = xml.Replace("zS_SZ5M7c", "FRfG1zYy");



            var xmlReaderSettings = new XmlReaderSettings() { CheckCharacters = false };
            var serializer = new DataContractSerializer(typeof(SerializationWrapper<T>));
            using (var sr = new StringReader(xml))
            {
                using (var reader = XmlReader.Create(sr, xmlReaderSettings))
                {
                    SerializationWrapper<T> data = new SerializationWrapper<T>();
                    try
                    {
                        data = (SerializationWrapper<T>)serializer.ReadObject(reader);
                        return data.Value;
                    }
                    catch
                    {
                        return data.Value;
                    }
                }
            }



        }
        public static Dictionary<string, ProcedureResultField> expandComplexFields(Dictionary<string, ProcedureResultField> input)
        {
            return input
                .Select(it =>
                {
                    if (it.Value is ProcedureComplexResultField)
                    {
                        ProcedureComplexResultField field = (ProcedureComplexResultField)it.Value;
                        return field.Fields
                            .Select(itc => new KeyValuePair<string, ProcedureResultField>(field.Name + "." + itc.Key, itc.Value))
                            .ToList();
                    }
                    else
                    {
                        return new List<KeyValuePair<string, ProcedureResultField>> { it };
                    }
                })
                .SelectMany(it => it)
                .ToDictionary(it => it.Key, it => it.Value);
        }
        //private void DeserializeObject(string xml)
        //{
        //    Console.WriteLine("Reading with XmlReader");

        //    // Create an instance of the XmlSerializer specifying type and namespace.
        //    XmlSerializer serializer = new XmlSerializer(typeof(Dictionary<string, string>));

        //    // A FileStream is needed to read the XML document.
        //    FileStream fs = new FileStream(xml, FileMode.Open);
        //    XmlReader reader = XmlReader.Create(fs);

        //    // Declare an object variable of the type to be deserialized.
        //    //OrderedItem i;

        //    //// Use the Deserialize method to restore the object's state.
        //    //Dictionary<string, ProcedureResultField> rezultaticici = Deserialize<Dictionary<string, ProcedureResultField>>(reader);

        //    fs.Close();

        //    // Write out the properties of the object.
        //    //Console.Write(
        //    //i.Name + "\t" +
        //    //i.Description + "\t"
        //    //);
        //}

        private static string ReplaceHexadecimalSymbols(this string txt)
        {
            string r = "[\x00-\x08\x0B\x0C\x0E-\x1F\x26]";
            return Regex.Replace(txt, r, "?", RegexOptions.Compiled);
        }

        [DataContract]
        private class SerializationWrapper<T> where T : new()
        {
            // need a parameterless constructor for serialization
            public SerializationWrapper()
            {
                Value = new T();
            }

            [DataMember]
            public T Value { get; set; }
        }


        static string RemoveInvalidXmlChars(string text)
        {
            var validXmlChars = text.Where(ch => XmlConvert.IsXmlChar(ch)).ToArray();
            return new string(validXmlChars);
        }

        static bool IsValidXmlString(string text)
        {
            try
            {
                XmlConvert.VerifyXmlChars(text);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
