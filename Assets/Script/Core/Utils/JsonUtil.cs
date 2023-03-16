using MiniJSON;
using System.Collections.Generic;

namespace FrameWork.Core.Utils
{
    public static class JsonUtil
    {
        public static bool TryDeserializeToListDictionary(string json, out List<Dictionary<string, string>> ret)
        {
            ret = default;
            var list = Json.Deserialize(json) as List<object>;
            if (list == null)
                return false;

            ret = new List<Dictionary<string, string>>();
            foreach (var item in list)
            {
                var temp = item as Dictionary<string, object>;
                if (temp != null)
                {
                    var keyValues = new Dictionary<string, string>();
                    foreach (var keyValue in temp)
                        keyValues.Add(keyValue.Key, (string)keyValue.Value);

                    ret.Add(keyValues);
                }
            }
            return true;
        }

        public static bool TryDeserializeToDictionary(string json, out Dictionary<string, Dictionary<string, string>> ret)
        {
            ret = default;
            var dic = Json.Deserialize(json) as Dictionary<string, object>;
            if (dic == null)
                return false;

            ret = new Dictionary<string, Dictionary<string, string>>();
            foreach (var item in dic)
            {
                var temp = item.Value as Dictionary<string, object>;
                if (temp != null)
                {
                    var keyValues = new Dictionary<string, string>();
                    foreach (var keyValue in temp)
                        keyValues.Add(keyValue.Key, (string)keyValue.Value);

                    ret.Add(item.Key, keyValues);
                }
            }
            return true;
        }

        public static bool TryDeserializeToDictionary(string json, out Dictionary<string, string> ret)
        {
            ret = default;
            var dic = Json.Deserialize(json) as Dictionary<string, object>;
            if (dic == null)
                return false;

            ret = new Dictionary<string, string>();
            foreach (var item in dic)
                ret.Add(item.Key, (string)item.Value);

            return true;
        }
    }
}