﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NetMiner.Common.Tool
{
    public class XMLUtil
    {
        /// <summary>
     /// 反序列化
     /// </summary>
     /// <param name="type">类型</param>
     /// <param name="xml">XML字符串</param>
     /// <returns></returns>
     public static object Deserialize(Type type, string xml)
     {
         try
         {
             using (StringReader sr = new StringReader(xml))
             {
                 XmlSerializer xmldes = new XmlSerializer(type);
                 return xmldes.Deserialize(sr);
             }
         }
         catch (Exception e)
         {
 
             return null;
         }
        }

        /// <summary>
     /// 序列化
     /// </summary>
     /// <param name="type">类型</param>
     /// <param name="obj">对象</param>
     /// <returns></returns>
     public static string Serializer(Type type, object obj)
     {
         MemoryStream Stream = new MemoryStream();
         XmlSerializer xml = new XmlSerializer(type);
         try
         {
             //序列化对象
             xml.Serialize(Stream, obj);
         }
         catch (InvalidOperationException)
         {
             throw;
         }
         Stream.Position = 0;
         StreamReader sr = new StreamReader(Stream);
         string str = sr.ReadToEnd();
          
         sr.Dispose();
         Stream.Dispose();
  
         return str;
     }
    }
}
