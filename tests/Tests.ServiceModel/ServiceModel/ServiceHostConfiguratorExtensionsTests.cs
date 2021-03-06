﻿using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using AutoFixture.Idioms;
using EMG.Utilities.ServiceModel;
using EMG.Utilities.ServiceModel.Configuration;
using EMG.Utilities.ServiceModel.Logging;
using Moq;
using NUnit.Framework;
using EndpointAddress = EMG.Utilities.ServiceModel.Configuration.EndpointAddress;

// ReSharper disable InvokeAsExtensionMethod

namespace Tests.ServiceModel
{
    [TestFixture]
    public class ServiceHostConfiguratorExtensionsTests
    {
        [Test, CustomAutoData]
        public void AddMetadataEndpoints_adds_service_host_configuration(WcfServiceHostConfiguration<TestService> configurator, Action<ServiceMetadataBehavior> serviceMetadataBehaviorConfigurator)
        {
            Assume.That(configurator.ServiceHostConfigurations, Is.Empty);

            ServiceHostConfiguratorExtensions.AddMetadataEndpoints(configurator, serviceMetadataBehaviorConfigurator);

            Assert.That(configurator.ServiceHostConfigurations, Has.One.InstanceOf<Action<ServiceHost>>());
        }

        [Test, CustomAutoData]
        public void AddMetadataEndpoints_adds_ServiceMetadataBehavior_to_host_with_NamedPipe_binding(WcfServiceHostConfiguration<TestService> configurator, ServiceHost host)
        {
            configurator.AddEndpoint<NetNamedPipeBinding>(typeof(ITestService), new Uri("net.pipe://localhost/test"));

            configurator.ConfigureServiceHost(host);

            ServiceHostConfiguratorExtensions.AddMetadataEndpoints(configurator);

            var configuration = configurator.ServiceHostConfigurations.First();

            configuration(host);

            Assert.That(host.Description.Behaviors.Find<ServiceMetadataBehavior>(), Is.Not.Null);
        }

        [Test, CustomAutoData]
        public void AddMetadataEndpoints_adds_metadata_endpoint_to_host_with_NamedPipe_binding(WcfServiceHostConfiguration<TestService> configurator, ServiceHost host)
        {
            configurator.AddEndpoint<NetNamedPipeBinding>(typeof(ITestService), new Uri("net.pipe://localhost/test"));

            configurator.ConfigureServiceHost(host);

            ServiceHostConfiguratorExtensions.AddMetadataEndpoints(configurator);

            var configuration = configurator.ServiceHostConfigurations.First();

            configuration(host);

            Assert.That(host.Description.Endpoints.Any(endpoint => endpoint.Address.Uri.AbsolutePath.EndsWith("mex")));
        }

        [Test, CustomAutoData]
        public void AddMetadataEndpoints_adds_ServiceMetadataBehavior_to_host_with_NetTcp_binding(WcfServiceHostConfiguration<TestService> configurator, ServiceHost host)
        {
            configurator.AddEndpoint<NetTcpBinding>(typeof(ITestService), new Uri("net.tcp://localhost/test"));

            configurator.ConfigureServiceHost(host);

            ServiceHostConfiguratorExtensions.AddMetadataEndpoints(configurator);

            var configuration = configurator.ServiceHostConfigurations.First();

            configuration(host);

            Assert.That(host.Description.Behaviors.Find<ServiceMetadataBehavior>(), Is.Not.Null);
        }

        [Test, CustomAutoData]
        public void AddMetadataEndpoints_adds_metadata_endpoint_to_host_with_NetTcp_binding(WcfServiceHostConfiguration<TestService> configurator, ServiceHost host)
        {
            configurator.AddEndpoint<NetTcpBinding>(typeof(ITestService), new Uri("net.tcp://localhost/test"));

            configurator.ConfigureServiceHost(host);

            ServiceHostConfiguratorExtensions.AddMetadataEndpoints(configurator);

            var configuration = configurator.ServiceHostConfigurations.First();

            configuration(host);

            Assert.That(host.Description.Endpoints.Any(endpoint => endpoint.Address.Uri.AbsolutePath.EndsWith("mex")));
        }

        [Test, CustomAutoData]
        public void AddMetadataEndpoints_adds_ServiceMetadataBehavior_to_host_with_BasicHttp_binding(WcfServiceHostConfiguration<TestService> configurator, ServiceHost host)
        {
            configurator.AddEndpoint<BasicHttpBinding>(typeof(ITestService), new Uri("http://localhost/test"));

            configurator.ConfigureServiceHost(host);

            ServiceHostConfiguratorExtensions.AddMetadataEndpoints(configurator);

            var configuration = configurator.ServiceHostConfigurations.First();

            configuration(host);

            Assert.That(host.Description.Behaviors.Find<ServiceMetadataBehavior>(), Is.Not.Null);
        }

        [Test, CustomAutoData]
        public void AddMetadataEndpoints_adds_metadata_endpoint_to_host_with_BasicHttp_binding(WcfServiceHostConfiguration<TestService> configurator, ServiceHost host)
        {
            configurator.AddEndpoint<BasicHttpBinding>(typeof(ITestService), new Uri("http://localhost/test"));

            configurator.ConfigureServiceHost(host);

            ServiceHostConfiguratorExtensions.AddMetadataEndpoints(configurator);

            var configuration = configurator.ServiceHostConfigurations.First();

            configuration(host);

            Assert.That(host.Description.Endpoints.Any(endpoint => endpoint.Address.Uri.AbsolutePath.EndsWith("mex")));
        }

        [Test, CustomAutoData]
        public void AddMetadataEndpoints_throws_if_not_supported_endpoint_is_added(WcfServiceHostConfiguration<TestService> configurator, ServiceHost host)
        {
            configurator.ConfigureServiceHost(host);

            ServiceHostConfiguratorExtensions.AddMetadataEndpoints(configurator);

            var configuration = configurator.ServiceHostConfigurations.First();

            Assert.Throws<InvalidOperationException>(() => configuration(host));
        }

        [Test, CustomAutoData]
        public void AddExecutionLogging_adds_service_host_configuration(WcfServiceHostConfiguration<TestService> configurator, Action<ServiceMetadataBehavior> serviceMetadataBehaviorConfigurator)
        {
            Assume.That(configurator.ServiceHostConfigurations, Is.Empty);

            ServiceHostConfiguratorExtensions.AddExecutionLogging(configurator);

            Assert.That(configurator.ServiceHostConfigurations, Has.One.InstanceOf<Action<ServiceHost>>());
        }

        [Test, CustomAutoData]
        public void AddExecutionLogging_adds_behavior_to_host(WcfServiceHostConfiguration<TestService> configurator, ServiceHost host)
        {
            ServiceHostConfiguratorExtensions.AddExecutionLogging(configurator);

            var configuration = configurator.ServiceHostConfigurations.First();

            configuration(host);

            Assert.That(host.Description.Behaviors.Find<ExecutionLoggingBehavior>(), Is.Not.Null);
        }

        [Test, CustomAutoData]
        public void AddBasicHttpEndpoint_forwards_to_configurator(IServiceHostConfigurator configurator, Action<BasicHttpBinding> testDelegate, HttpEndpointAddress address)
        {
            configurator.AddBasicHttpEndpoint(typeof(ITestService), address, testDelegate);

            Mock.Get(configurator).Verify(p => p.AddEndpoint<BasicHttpBinding>(typeof(ITestService), address, testDelegate));
        }

        [Test, CustomAutoData]
        public void AddSecureBasicHttpEndpoint_forwards_to_configurator(IServiceHostConfigurator configurator, Action<BasicHttpBinding> testDelegate, string host, string path, int port)
        {
            var address = EndpointAddress.ForHttp(host, path, port, false);

            configurator.AddSecureBasicHttpEndpoint(typeof(ITestService), address, testDelegate);

            Mock.Get(configurator).Verify(p => p.AddEndpoint<BasicHttpBinding>(typeof(ITestService), It.Is<HttpEndpointAddress>(a => a.IsSecure && a.Host == host && a.Port == port && a.Path == path), It.IsAny<Action<BasicHttpBinding>>()));
        }

        [Test, CustomAutoData]
        public void AddWSHttpEndpoint_forwards_to_configurator(IServiceHostConfigurator configurator, Action<WSHttpBinding> testDelegate, HttpEndpointAddress address)
        {
            configurator.AddWSHttpEndpoint(typeof(ITestService), address, testDelegate);

            Mock.Get(configurator).Verify(p => p.AddEndpoint<WSHttpBinding>(typeof(ITestService), address, testDelegate));
        }

        [Test, CustomAutoData]
        public void AddSecureWSHttpEndpoint_forwards_to_configurator(IServiceHostConfigurator configurator, Action<WSHttpBinding> testDelegate, string host, string path, int port)
        {
            var address = EndpointAddress.ForHttp(host, path, port, false);

            configurator.AddSecureWSHttpEndpoint(typeof(ITestService), address, testDelegate);

            Mock.Get(configurator).Verify(p => p.AddEndpoint<WSHttpBinding>(typeof(ITestService), It.Is<HttpEndpointAddress>(a => a.IsSecure && a.Host == host && a.Port == port && a.Path == path), It.IsAny<Action<WSHttpBinding>>()));
        }

        [Test, CustomAutoData]
        public void AddNetTcpEndpoint_forwards_to_configurator(IServiceHostConfigurator configurator, Action<NetTcpBinding> testDelegate, NetTcpEndpointAddress address)
        {
            configurator.AddNetTcpEndpoint(typeof(ITestService), address, testDelegate);

            Mock.Get(configurator).Verify(p => p.AddEndpoint<NetTcpBinding>(typeof(ITestService), address, testDelegate));
        }

        [Test, CustomAutoData]
        public void AddNamedPipeEndpoint_forwards_to_configurator(IServiceHostConfigurator configurator, Action<NetNamedPipeBinding> testDelegate, NamedPipeEndpointAddress address)
        {
            configurator.AddNamedPipeEndpoint(typeof(ITestService), address, testDelegate);

            Mock.Get(configurator).Verify(p => p.AddEndpoint<NetNamedPipeBinding>(typeof(ITestService), address, testDelegate));
        }

        [Test, CustomAutoData]
        public void ConfigureService_alters_ServiceBehaviorAttribute(WcfServiceHostConfiguration<TestService> configurator, Action<ServiceBehaviorAttribute> testDelegate, ServiceHost serviceHost)
        {
            configurator.ConfigureService(testDelegate);

            configurator.ConfigureServiceHost(serviceHost);

            Mock.Get(testDelegate).Verify(p => p(serviceHost.Description.Behaviors.Find<ServiceBehaviorAttribute>()));
        }

        [Test, CustomAutoData]
        public void ConfigureService_alters_ServiceBehaviorAttribute(WcfServiceHostConfiguration<TestService> configurator, ServiceHost serviceHost, AddressFilterMode filterMode)
        {
            configurator.ConfigureService(service => service.AddressFilterMode = filterMode);

            configurator.ConfigureServiceHost(serviceHost);

            Assert.That(serviceHost.Description.Behaviors.Find<ServiceBehaviorAttribute>().AddressFilterMode, Is.EqualTo(filterMode));
        }

        [Test, CustomAutoData]
        public void ConfigureService_checks_for_nulls(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(ServiceHostConfiguratorExtensions).GetMethod(nameof(ServiceHostConfiguratorExtensions.ConfigureService)));
        }

        [Test, CustomAutoData]
        public void AcceptAllIncomingRequests_sets_filterMode_to_Any(WcfServiceHostConfiguration<TestService> configurator, ServiceHost serviceHost)
        {
            configurator.AcceptAllIncomingRequests();

            configurator.ConfigureServiceHost(serviceHost);

            Assert.That(serviceHost.Description.Behaviors.Find<ServiceBehaviorAttribute>().AddressFilterMode, Is.EqualTo(AddressFilterMode.Any));
        }

        [Test, CustomAutoData]
        public void AddExecutionLogging_checks_for_nulls(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(ServiceHostConfiguratorExtensions).GetMethod(nameof(ServiceHostConfiguratorExtensions.AddExecutionLogging)));
        }
    }
}