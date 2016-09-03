using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;

namespace WcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        #region GETService

        #region SimpleGETService

        [OperationContract]
        //Also use as WebInvoke For GET Service
        //[WebInvoke(Method="GET",UriTemplate = "SimpleGet?", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [WebGet(UriTemplate = "SimpleGet?",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        string SimpleGet();//http://localhost:53775/Service1.svc/SimpleGet?

        [OperationContract]
        [WebGet(UriTemplate = "SimpleGetWithParameter?Input={input}",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        string SimpleGetwithParameter(string input);//http://localhost:53775/Service1.svc/SimpleGetWithParameter?input=test

        [OperationContractAttribute(AsyncPattern = true)]
        [WebGet(UriTemplate = "SimpleGetTaskBaseAsyncService?",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Task<string> SimpleGetTaskBaseAsyncService();//http://localhost:53775/Service1.svc/SimpleGetTaskBaseAsyncService?

        [OperationContractAttribute(AsyncPattern = true)]
        [WebGet(UriTemplate = "SimpleGetTaskBaseAsyncServiceWithParameter?Input={input}",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Task<string> SimpleGetTaskBaseAsyncServiceWithParameter(string input);//http://localhost:53775/Service1.svc/SimpleGetTaskBaseAsyncServiceWithParameter?input=test

        #endregion

        #endregion

        #region POSTService

        #region SimplePOSTService

        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "SimplePost?",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        string SimplePost();

        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "SimplePostWithParameter?Input={input}",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        string SimplePostWithParameter(string input);


        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "SimplePostTaskBaseAsyncService?",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Task<string> SimplePostTaskBaseAsyncService();

        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "SimplePostTaskBaseAsyncServiceWithParameter?Input={input}",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Task<string> SimplePostTaskBaseAsyncServiceWithParameter(string input);


        #endregion

        #region PostWithObj

        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "PostWithObj?",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Task<PostObj> PostWithObj(PostObj obj);
        //Service url :==>http://localhost:53775/Service1.svc/PostWithObj?
        //raw Data:==> {"input1":"test","input2":2,"input3":"true"}
        #endregion

        #region PostWithList

        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "PostWithList?",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Task<List<PostObj>> PostWithList(List<PostObj> list);
        //Service url :==>http://localhost:53775/Service1.svc/PostWithList?
        //raw Data:==> [{"input1":"test","input2":2,"input3":"true"},{"input1":"test2","input2":3,"input3":"false"}]
        #endregion

        #region PostUploadImageOrVideo

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "Upload?Type={type}", BodyStyle = WebMessageBodyStyle.Bare,
           RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Task<UploadPhoto> Upload(string type, Stream str);

        #endregion

        #endregion

        //[OperationContract]
        //[WebInvoke(Method = "POST", UriTemplate = "UploadPhoto1?Ostype={ostype}&Type={type}", BodyStyle = WebMessageBodyStyle.Bare,
        //   RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        //Task<UploadPhoto> UploadPhoto1(string ostype, string type, Stream str);
    }

    #region UploadPhoto
    [DataContract]
    public class UploadPhoto
    {
        [DataMember]
        public bool flag { get; set; }
        [DataMember]
        public string Message { get; set; }
    }
    #endregion

    #region PostObj
    [DataContract]
    public class PostObj
    {
        [DataMemberAttribute(Order = 0)]
        public string input1 { get; set; }

        [DataMemberAttribute(Order=1)]
        public int input2 { get; set; }

        [DataMemberAttribute(Order = 2)]
        public bool input3 { get; set; }
    }
    #endregion
}
