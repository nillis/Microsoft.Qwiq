using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

using Microsoft.Qwiq;
using Microsoft.Qwiq.Core.Tests;
using Microsoft.Qwiq.Identity.Mapper;
using Microsoft.Qwiq.Mapper;
using Microsoft.Qwiq.Mapper.Attributes;
using Microsoft.Qwiq.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Qwiq.Benchmark;
using Qwiq.Identity.Tests.Mocks;
using Should;

namespace Qwiq.Identity.Tests
{
    public abstract class BulkIdentityAwareAttributeMapperStrategyTests : ContextSpecification
    {
        private IWorkItemMapperStrategy _strategy;
        private IEnumerable<KeyValuePair<IWorkItem, IIdentifiable>> _workItemMappings;
        protected IDictionary<string, IEnumerable<ITeamFoundationIdentity>> Identities { get; set; }

        protected MockIdentityType Actual
        {
            get { return _workItemMappings.Select(kvp => kvp.Value).Cast<MockIdentityType>().Single(); }
        }

        protected string IdentityFieldBackingValue { get; set; }

        public override void Given()
        {
            var propertyInspector = new PropertyInspector(new PropertyReflector());
            _strategy = new BulkIdentityAwareAttributeMapperStrategy(
                            propertyInspector,
                            Identities == null
                                ? new MockIdentityManagementService()
                                : new MockIdentityManagementService(Identities));
            var sourceWorkItems = new[]
            {
                new MockWorkItem(new Dictionary<string, object>
                    {
                        { MockIdentityType.BackingField, IdentityFieldBackingValue }
                    })
            };

            _workItemMappings = sourceWorkItems.Select(t => new KeyValuePair<IWorkItem, IIdentifiable>(t, new MockIdentityType())).ToList();
        }

        public override void When()
        {
            _strategy.Map(typeof (MockIdentityType), _workItemMappings, null);
        }
    }

    [TestClass]
    public class when_a_backing_source_is_the_empty_string_the_field_should_not_be_mapped : BulkIdentityAwareAttributeMapperStrategyTests
    {
        public override void Given()
        {
            IdentityFieldBackingValue = string.Empty;
            base.Given();
        }

        [TestMethod]
        public void the_actual_identity_value_should_be_null()
        {
            Actual.AnIdentity.ShouldBeNull();
        }
    }

    [TestClass]
    public class when_the_backing_source_does_not_result_in_a_resolved_identity : BulkIdentityAwareAttributeMapperStrategyTests
    {
        public override void Given()
        {
            IdentityFieldBackingValue = "Anon Existant";
            Identities = new Dictionary<string, IEnumerable<ITeamFoundationIdentity>>();
            base.Given();
        }

        [TestMethod]
        public void the_actual_identity_value_should_be_null()
        {
            Actual.AnIdentity.ShouldBeNull();
        }

        [TestMethod]
        public void set_on_an_identity_property_should_not_be_called()
        {
            Actual.AnIdentitySetCount.ShouldEqual(0);
        }
    }

    [TestClass]
    public class when_the_backing_source_does_result_in_a_resolved_identity : BulkIdentityAwareAttributeMapperStrategyTests
    {
        private const string identityAlias = "jsmit";
        private const string identityDisplay = "Joe Smit";
        public override void Given()
        {
            IdentityFieldBackingValue = identityDisplay;
            Identities = new Dictionary<string, IEnumerable<ITeamFoundationIdentity>>
            {
                {IdentityFieldBackingValue, new []{new MockTeamFoundationIdentity(identityDisplay, identityAlias) }}
            };
            base.Given();
        }

        [TestMethod]
        public void the_actual_identity_value_should_be_the_identity_alias()
        {
            Actual.AnIdentity.ShouldEqual(identityAlias);
        }

        [TestMethod]
        public void set_on_an_identity_property_should_be_called_once()
        {
            Actual.AnIdentitySetCount.ShouldEqual(1);
        }
    }

    [TestClass]
    public class given_a_work_item_with_defined_fields_when_the_field_names_to_properties_are_retrieved : ContextSpecification
    {
        private readonly Type _identityType = typeof (MockIdentityType);
        private IDictionary<string, PropertyInfo> Expected { get; set; }
        private IDictionary<string, PropertyInfo> Actual { get; set; }

        public override void Given()
        {
            Expected = new Dictionary<string, PropertyInfo>
            {
                { MockIdentityType.BackingField, _identityType.GetProperty("AnIdentity") },
                { MockIdentityType.NonExistantField, _identityType.GetProperty("NonExistant") },
                { MockIdentityType.UriIdentityField, _identityType.GetProperty("UriIdentity") }
            };
        }

        public override void When()
        {
            Actual =
                BulkIdentityAwareAttributeMapperStrategy.GetWorkItemIdentityFieldNameToIdentityPropertyMap(
                    _identityType, new PropertyInspector(new PropertyReflector()));
            base.When();
        }

        [TestMethod]
        public void only_valid_fields_and_properties_are_retrieved()
        {
            Actual.ShouldContainOnly(Expected);
        }
    }

    [Config(typeof(BenchmarkConfig))]
    [TestClass]
    public class Benchmark
    {
        private IWorkItemMapperStrategy _strategy;
        private IEnumerable<KeyValuePair<IWorkItem, IIdentifiable>> _workItemMappings;

        [Setup]
        [TestInitialize]
        public void Setup()
        {
            var propertyInspector = new PropertyInspector(new PropertyReflector());
            _strategy = new BulkIdentityAwareAttributeMapperStrategy(
                            propertyInspector,
                            new MockIdentityManagementService()
                        );

            var generator = new WorkItemGenerator<MockWorkItem>(()=> new MockWorkItem(), new[] { "Revisions", "Item", "AssignedTo" });
            generator.Generate();

            var assignees = new[]
                                {
                                    MockIdentityManagementService.Danj.DisplayName,
                                    MockIdentityManagementService.Adamb.DisplayName,
                                    MockIdentityManagementService.Chrisj.DisplayName,
                                    MockIdentityManagementService.Chrisjoh.DisplayName,
                                    MockIdentityManagementService.Chrisjohn.DisplayName,
                                    MockIdentityManagementService.Chrisjohns.DisplayName
                                };

            var sourceWorkItems = generator
                                    .Items
                                    // Run post-randomization to enable our scenario
                                    .Select(
                                        s =>
                                            {
                                                var i = Randomizer.Instance.Next(0, assignees.Length - 1);
                                                s[MockIdentityType.BackingField] = assignees[i];

                                                return s;
                                            });

            _workItemMappings = sourceWorkItems.Select(t => new KeyValuePair<IWorkItem, IIdentifiable>(t, new MockIdentityType())).ToList();
        }

        [Benchmark]
        public IEnumerable<KeyValuePair<IWorkItem, IIdentifiable>> Execute()
        {
            _strategy.Map(typeof(MockIdentityType), _workItemMappings, null);
            return _workItemMappings;
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("Performance")]
        [TestCategory("Benchmark")]
        public void Execute_Identity_Mapping_Performance_Benchmark()
        {
            BenchmarkRunner.Run<Benchmark>();
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("Performance")]
        public void Execute_Identity_Mapping()
        {
            Execute();
        }
    }
}

