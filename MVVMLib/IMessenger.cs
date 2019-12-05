using System;

namespace MVVMLib
{
    /// <summary>
    /// 发布-订阅消息代理的接口
    /// </summary>
    public interface IMessenger
    {
        /// <summary>
        /// 发布一个消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        void Publish<T>(T message);

        /// <summary>
        /// 订阅者注册一个回调，在收到指定类型T的消息后执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subscriber"></param>
        /// <param name="callback"></param>
        void Subscribe<T>(object subscriber, Action<T> callback);

        /// <summary>
        /// 取消订阅者的所有订阅
        /// </summary>
        /// <param name="subscriber"></param>
        void Unsubscribe(object subscriber);
    }
}
