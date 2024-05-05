using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Eldan.TypeExtensions
{
    public static class ServiceModelExtentions
    {
        public static T GetProxy<T>(string uri)
        {
            return GetProxy<T>(uri, null, null, null, null);
        }

        public static T GetProxy<T>(string uri, TimeSpan? closeTimeout, TimeSpan? openTimeout, TimeSpan? receiveTimeout, TimeSpan? sendTimeout)
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();

            if (closeTimeout != null)
                basicHttpBinding.CloseTimeout = closeTimeout.Value;

            if (openTimeout != null)
                basicHttpBinding.OpenTimeout = openTimeout.Value;

            if (receiveTimeout != null)
                basicHttpBinding.ReceiveTimeout = receiveTimeout.Value;

            if (sendTimeout != null)
                basicHttpBinding.SendTimeout = sendTimeout.Value;

            EndpointAddress address = new EndpointAddress(uri);

            ChannelFactory<T> factory = new ChannelFactory<T>(basicHttpBinding, address);
            T proxy = factory.CreateChannel();

            return proxy;

        }
    }
}
