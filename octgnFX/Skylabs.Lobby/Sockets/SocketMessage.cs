﻿using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Skylabs.Net
{
    [Serializable]
    public class NameValuePair
    {
        public string Key { get; set; }

        public object Value { get; set; }

        public NameValuePair()
        {
            Key = "";
            Value = "";
        }

        public NameValuePair(string key)
        {
            Key = key;
            Value = "";
        }

        public NameValuePair(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public NameValuePair(string key, object value)
        {
            Key = key;
            Value = value;
        }
    }

    [Serializable]
    public class SocketMessage
    {
        public NameValuePair[] Data { get { return _Data; } set { _Data = value; } }

        public string Header { get; set; }

        private NameValuePair[] _Data;

        public SocketMessage(string header)
        {
            Header = header;
            Data = new NameValuePair[0];
        }

        public object this[string key]
        {
            get
            {
                for(int i=0; i < _Data.Length; i++)
                {
                    if(_Data[i].Key == key)
                        return _Data[i].Value;
                }
                return null;
            }
            set
            {
                for(int i=0; i < _Data.Length; i++)
                {
                    if(_Data[i].Key == key)
                        _Data[i].Value = value;
                }
            }
        }

        public void Add_Data(NameValuePair data)
        {
            Array.Resize<NameValuePair>(ref _Data, Data.Length + 1);
            _Data[_Data.Length - 1] = data;
        }

        public void Add_Data(string key, object value)
        {
            Array.Resize<NameValuePair>(ref _Data, Data.Length + 1);
            _Data[_Data.Length - 1] = new NameValuePair(key, value);
        }

        public static byte[] Serialize(SocketMessage message)
        {
            using(MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, message);
                ms.Flush();
                return ms.ToArray();
            }
        }

        public static SocketMessage Deserialize(byte[] data)
        {
            using(MemoryStream ms = new MemoryStream(data))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return (SocketMessage)bf.Deserialize(ms);
            }
        }
    }
}