using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.sc;
using System.Reflection;
//using com.mirle.ibg3k0.ohxc.winform.Service;

namespace com.mirle.ibg3k0.sc.Common
{
    public class ElasticSearchManager
    {

        public const string ELASTIC_URL = "elastic.viewer.mirle.com.tw";
        //[LinkObject(nameof(Service.ASYSEXCUTEQUALITY) , nameof(sysexcutequality))] 
        //public const string ELASTIC_TABLE_INDEX_SYSEXCUTEQUALITY = "mfoht100-ohtc1-sysexcutequality";
        //[LinkObject(nameof(RecordReportInfo))] 
        //public const string ELASTIC_TABLE_INDEX_RECODEREPORTINFO = "mfoht100-ohtc1-recodereportinfo";

        [LinkObject(nameof(PortDef))]
        public const string ELASTIC_TABLE_INDEX_PLCHistory = "mfoht100-ohtc1-plchistory";
        [LinkObject(nameof(AVEHICLE))]
        public const string ELASTIC_TABLE_INDEX_OHTHistory = "mfoht100-ohtc1-ohthistory";
        [LinkObject(nameof(ALARM))]
        public const string ELASTIC_TABLE_INDEX_AlarmHistory = "mfoht100-ohtc1-alarmhistory";
        [LinkObject(nameof(ACMD_MCS))]
        public const string ELASTIC_TABLE_INDEX_TransferCommandHistory = "mfoht100-ohtc1-transfercommandhistory";
        [LinkObject(nameof(ACMD_OHTC_DETAIL))]
        public const string ELASTIC_TABLE_INDEX_TaskCommandHistory = "mfoht100-ohtc1-taskcommandhistory";

        private static ElasticSearchManager instance = null;
        private static Object _lock = new Object();
        private ElasticSearchManager() { }
        public static ElasticSearchManager getInstance()
        {
            lock (_lock)
            {
                if (instance == null)
                {
                    instance = new ElasticSearchManager();
                }
                return instance;
            }
        }

        public List<T> Search<T>(DateRangeQuery dq, TermsQuery[] tsqs, string[] includes_column, int start_index, int each_search_size)
            where T : class, new()
        {
            T t = new T();
            var node = new Uri($"http://{ELASTIC_URL}:9200");
            var settings = new ConnectionSettings(node).DefaultIndex("default");
            settings.DisableDirectStreaming();
            var client = new ElasticClient(settings);

            var index = GetType().GetFields(BindingFlags.Public | BindingFlags.Static);
            var TName = t.GetType().Name;
            var index_tabel = index.Where(x => (x.GetCustomAttribute(typeof(LinkObject), false) as LinkObject).type.Contains(TName))
                .FirstOrDefault()?.GetValue(this);

            SearchRequest sr = new SearchRequest($"{index_tabel}*");
            sr.From = start_index;
            sr.Size = each_search_size;

            if (tsqs != null)
            {
                foreach (var tsq in tsqs)
                {
                    if (tsq != null)
                        sr.Query &= tsq;
                }
            }
            sr.Query &= dq;
            sr.Source = new SourceFilter()
            {
                Includes = includes_column,
            };
            var result = client.Search<T>(sr);
            return result.Documents.ToList();
        }

        public List<T> Search<T>(DateRangeQuery dq, TermsQuery[] tsqs, int start_index, int each_search_size)
           where T : class, new()
        {
            T t = new T();
            var node = new Uri($"http://{ELASTIC_URL}:9200");
            var settings = new ConnectionSettings(node).DefaultIndex("default");
            settings.DisableDirectStreaming();
            var client = new ElasticClient(settings);

            var index = GetType().GetFields(BindingFlags.Public | BindingFlags.Static);
            var TName = t.GetType().Name;
            var index_tabel = index.Where(x => (x.GetCustomAttribute(typeof(LinkObject), false) as LinkObject).type.Contains(TName))
                .FirstOrDefault()?.GetValue(this);

            SearchRequest sr = new SearchRequest($"{index_tabel}*");
            sr.From = start_index;
            sr.Size = each_search_size;
            var tmpPropertiesAry = typeof(T).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            if (tsqs != null)
            {
                foreach (var tsq in tsqs)
                {
                    if (tsq != null)
                        sr.Query &= tsq;
                }
            }
			if (dq != null)
			{
                sr.Query &= dq;
			}
            sr.Source = new SourceFilter()
            {
                Includes = tmpPropertiesAry,
            };
            var result = client.Search<T>(sr);
            return result.Documents.ToList();
        }

        public bool insertLogData<T>(T item) where T : class
        {
            var node = new Uri($"http://{ELASTIC_URL}:9200");
            var settings = new ConnectionSettings(node).DefaultIndex("default");
            settings.DisableDirectStreaming();
            var client = new ElasticClient(settings);

            var index = GetType().GetFields(BindingFlags.Public | BindingFlags.Static);
            var TName = item.GetType().Name;
            var index_tabel = index.Where(x => ((x.GetCustomAttribute(typeof(LinkObject), false) as LinkObject)?.type ?? new string[] { "" }).Contains(TName))
                .FirstOrDefault()?.GetValue(this);
            if (string.IsNullOrEmpty((string)index_tabel))
            {
                return false;
            }
            client.Index(item, i => i.Index($"{index_tabel}"));
            return true;
        }
    }

    class LinkObject : Attribute
    {
        public string[] type;
        public LinkObject(params string[] _type)
        {
            if (_type == null)
            {
                type = new string[] { "" };
                return;
            }
            type = new string[_type.Length];
            type = _type;
        }
    }
}
