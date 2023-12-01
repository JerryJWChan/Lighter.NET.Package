using System;
using System.Collections.Generic;
using System.Text;

namespace Lighter.NET.Common
{
    /// <summary>
    /// (抽象類別)回傳給Ajax call和WebApi call的結果物件
    /// </summary>	
     public abstract class ApiResult
    {
        /// <summary>
        /// 除錯用的訊息(***資安：不可顯示給client***)
        /// </summary>
        private List<Exception> _exceptions = new List<Exception>();

        /// <summary>
        /// 執行結果是否成功
        /// </summary>
        public virtual bool Success { get; protected set; }
        /// <summary>
        /// 執行結果是否失敗
        /// </summary>
        public bool Failed { get { return !Success; } }
        /// <summary>
        /// Http Status Code
        /// </summary>
        public virtual int StatusCode { get; set; }
        /// <summary>
        /// 訊息(可多筆)(可顯示給client)
        /// </summary>
        public List<MessageModel> Messages { get; set; } = new List<MessageModel>();
        /// <summary>
        /// data model檢核錯誤訊息
        /// </summary>
        public List<ModelError> ModelErrors { get; set; } = new List<ModelError>();
        /// <summary>
        /// 回傳的資料(通常是data model)
        /// </summary>
        public virtual object? Data { get; set; }
        /// <summary>
        /// 回傳資料的詮釋資料
        /// </summary>
        public object? MetaData { get; set; }
        /// <summary>
        /// Client端是否需要跳窗顯示訊息
        /// </summary>
        public bool IsPopup { get; set; } = false;
        /// <summary>
        /// 是否有例外
        /// </summary>
        public bool HasException
        {
            get { return _exceptions.Count > 0; }
        }
        /// <summary>
        /// 加入例外
        /// </summary>
        /// <param name="ex"></param>
        public void AddException(Exception ex)
        {
            _exceptions.Add(ex);
            Success= false;
        }

        /// <summary>
        /// 例外集合
        /// </summary>
        /// <returns></returns>
        public List<Exception> Exceptions()
        {
            return _exceptions;
        }
    }

    /// <summary>
    /// 回傳[成功]結果給Ajax call和WebApi call的容器物件
    /// </summary>
    public class ApiSuccessResult : ApiResult
    {
        public override bool Success => true;

        public override int StatusCode { get; set; } = 200;
        /// <summary>
        /// 成功結果
        /// </summary>
        public ApiSuccessResult() { }

        /// <summary>
        /// 成功結果(帶回data)
        /// </summary>
        /// <param name="data">通常是帶回data model</param>
        /// <param name="message">執行成功訊息</param>
        /// <param name="messageCaption">訊息標頭</param>
        public ApiSuccessResult(object? data,string message="",string messageCaption="")
        {
            Data = data;
            Messages.Add(new MessageModel() { Type = MessageType.Info, Text = message, Caption= messageCaption });
        }
    }

    /// <summary>
    /// 回傳[失敗]結果給Ajax call和WebApi call的容器物件
    /// </summary>
    public class ApiFailResult : ApiResult
    {
        public override bool Success => false; 
        public override int StatusCode { get; set; } = 500;
        /// <summary>
        /// 失敗結果
        /// </summary>
        public ApiFailResult() { }

        /// <summary>
        /// 失敗結果(帶回vmr中的modelErrors和訊息)
        /// </summary>
        /// <param name="vmr"></param>
        public ApiFailResult(ViewModelWrapper vmr)
        {
            if(vmr.ModelErrors != null && vmr.ModelErrors.Count > 0)
            {
                ModelErrors.AddRange(vmr.ModelErrors);
            }
            if(vmr.Messages != null && vmr.Messages.Count > 0)
            {
                Messages.AddRange(vmr.Messages);
            }
        }
        /// <summary>
        /// 失敗結果(帶回modelErrors)
        /// </summary>
        /// <param name="modelErrors"></param>
        public ApiFailResult(IList<ModelError> modelErrors) 
        {
             ModelErrors.AddRange(modelErrors);
        }

        /// <summary>
        /// 失敗結果(帶回訊息)
        /// </summary>
        /// <param name="message">錯誤訊息</param>
        /// <param name="messageType">錯誤訊息類別：預設Error</param>
        /// <param name="messageCaption">訊息標頭</param>
        public ApiFailResult(string message, MessageType messageType = MessageType.Error, string messageCaption="") 
        {
            Messages.Add(new MessageModel() { Type = messageType, Text = message, Caption = messageCaption });
        }

        /// <summary>
        /// 失敗結果(帶回model error)
        /// </summary>
        /// <param name="modelErrors">data model檢核錯誤</param>
        /// <param name="message">錯誤訊息</param>
        /// <param name="messageType">錯誤訊息類別：預設Error</param>
        /// <param name="messageCaption">訊息標頭</param>
        public ApiFailResult(IList<ModelError> modelErrors, string message , MessageType messageType = MessageType.Error, string messageCaption = "") 
        {
            ModelErrors.AddRange(modelErrors);
            Messages.Add(new MessageModel() { Type = messageType, Text = message, Caption = messageCaption });
        }

        /// <summary>
        /// 失敗結果(帶回訊息)
        /// </summary>
        /// <param name="messages">訊息清單</param>
        public ApiFailResult(MessageModel messages)
        {
            if (messages == null) return;
            List<MessageModel> messagesList = new List<MessageModel>() { messages };
            Messages.AddRange(messagesList);
        }

        /// <summary>
        /// 失敗結果(帶回訊息清單)
        /// </summary>
        /// <param name="messages">訊息清單</param>
        public ApiFailResult(IList<MessageModel> messages)
        {
            if (messages == null || messages.Count == 0) return;
            Messages.AddRange(messages);
        }
    }

    /// <summary>
    /// (泛型)回傳給Ajax call和WebApi call的結果物件
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class ApiResult<TData>:ApiResult 
    {
        /// <summary>
        /// Api執行結果
        /// </summary>
        public ApiResult() { }
        public new TData? Data { get; set; }
    }

    /// <summary>
    /// 回傳[成功]結果給Ajax call和WebApi call的容器物件
    /// </summary>
    public class ApiSuccessResult<TData> : ApiResult<TData>
    {
        public override bool Success => true;

        public override int StatusCode { get; set; } = 200;
        /// <summary>
        /// 成功結果
        /// </summary>
        public ApiSuccessResult() { }
        /// <summary>
        /// 成功結果(帶回data)
        /// </summary>
        /// <param name="data">通常是帶回data model</param>
        /// <param name="message">執行成功訊息</param>
        /// <param name="messageCaption">訊息標頭</param>
        public ApiSuccessResult(TData? data, string message = "", string messageCaption = "")
        {
            Data = data;
            Messages.Add(new MessageModel() { Type = MessageType.Info, Text = message, Caption = messageCaption });
        }
    }

    /// <summary>
    /// 回傳[失敗]結果給Ajax call和WebApi call的容器物件
    /// </summary>
    public class ApiFailResult<TData> : ApiResult<TData> 
    {
        public override bool Success => false;
        public override int StatusCode { get; set; } = 500;
        /// <summary>
        /// 失敗結果
        /// </summary>
        public ApiFailResult() { }

        /// <summary>
        /// 失敗結果
        /// </summary>
        /// <param name="modelErrors"></param>
        public ApiFailResult(IList<ModelError> modelErrors)
        {
            ModelErrors.AddRange(modelErrors);
        }

        /// <summary>
        /// 失敗結果(帶回訊息)
        /// </summary>
        /// <param name="message">錯誤訊息</param>
        /// <param name="messageType">錯誤訊息類別：預設Error</param>
        /// <param name="messageCaption">訊息標頭</param>
        public ApiFailResult(string message, MessageType messageType = MessageType.Error, string messageCaption = "")
        {
            Messages.Add(new MessageModel() { Type = messageType, Text = message, Caption = messageCaption });
        }

        /// <summary>
        /// 失敗結果(帶回model error)
        /// </summary>
        /// <param name="modelErrors">data model檢核錯誤</param>
        /// <param name="message">錯誤訊息</param>
        /// <param name="messageType">錯誤訊息類別：預設Error</param>
        /// <param name="messageCaption">訊息標頭</param>
        public ApiFailResult(IList<ModelError> modelErrors, string message, MessageType messageType = MessageType.Error, string messageCaption = "")
        {
            ModelErrors.AddRange(modelErrors);
            Messages.Add(new MessageModel() { Type = messageType, Text = message, Caption = messageCaption });
        }
        /// <summary>
        /// 失敗結果(帶回訊息清單)
        /// </summary>
        /// <param name="messages">訊息清單</param>
        public ApiFailResult(IList<MessageModel> messages)
        {
            if (messages == null || messages.Count == 0) return;
            Messages.AddRange(messages);
        }
    }

}
