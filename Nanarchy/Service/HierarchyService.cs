using System;
using System.Collections.Generic;
using System.Linq;
using Nanarchy.Core;
using Nanarchy.Core.Interfaces;
using Nanarchy.Data.Mssql;

namespace Nanarchy.Service
{
    public class HierarchyService : IHierarchyService
    {
        private readonly IHierarchyDataProvider hierarchyEntryDataProvider;
        private readonly IHierarchyEntryRepository hierarchyEntryEntryRepository;
        private readonly ITargetEntryRepository _targetEntryRepository;
        private readonly ITargetRepository _targetRepository;
        private readonly List<Type> _targetTypes;
        private string _schemaName;

        public HierarchyService(
            IHierarchyDataProvider hierarchyDataProvider, 
            IHierarchyEntryRepository  hierarchyEntryRepository,
            ITargetEntryRepository targetEntryRepository,
            ITargetRepository targetRepository,
            List<Type> targetTypes)
        {
            hierarchyEntryDataProvider = hierarchyDataProvider;
            hierarchyEntryEntryRepository = hierarchyEntryRepository;
            _targetEntryRepository = targetEntryRepository;
            _targetRepository = targetRepository;
            _targetTypes = targetTypes;
        }

        #region Initialization Methods - used on startup


        public void Initialize()
        {
            hierarchyEntryEntryRepository.Initialize();
            _targetEntryRepository.Initialize();
            

            if (_targetTypes == null || !_targetTypes.Any()) return;

            foreach (var targetType in _targetTypes)
            {
                var target = (ITarget) Activator.CreateInstance(targetType);
                var tableName = target.TableName;
                _targetRepository.Initialize(tableName);
            }
        }

        #endregion

        public HierarchyNode InitializeHierarchy(HierarchyEntry hierarchyEntry, ITarget rootTarget = null)
        {
            var rootNode = new HierarchyNode
            {
                LeftId = 1,
                RightId = 2,
                TargetId = rootTarget == null ? 0 : rootTarget.Id 
            };

            // initialize the target table for the node
            hierarchyEntryDataProvider.Add(hierarchyEntry, rootNode);

            return rootNode;
        }

        public HierarchyNode GetRootNode(HierarchyEntry hierarchyEntry)
        {
            return hierarchyEntryDataProvider.GetRootNode(hierarchyEntry);
        }

        public HierarchyNode InsertNode(HierarchyEntry hierarchyEntry, HierarchyNode parentNode, ITarget childTarget)
        {
            var childNode = new HierarchyNode { TargetId = childTarget.Id };
            childNode = PrepareForInsertNode(hierarchyEntry, parentNode, childNode);
            hierarchyEntryDataProvider.Add(hierarchyEntry, childNode);
            return childNode;
        }

        public HierarchyNode PrepareForInsertNode(HierarchyEntry hierarchyEntry, HierarchyNode parentNode, HierarchyNode childNode)
        {
            childNode.LeftId = parentNode.RightId;
            hierarchyEntryDataProvider.PrepareForInsertNode(hierarchyEntry, parentNode);
            childNode.RightId = childNode.LeftId + 1;
            return childNode;
        }

        public void DeleteNode(HierarchyEntry hierarchyEntry, HierarchyNode node)
        {
            hierarchyEntryDataProvider.Delete(hierarchyEntry, node);
        }

        public IList<HierarchyNode> GetChildren(HierarchyEntry hierarchyEntry, HierarchyNode parentNode)
        {
            return hierarchyEntryDataProvider.GetChildren(hierarchyEntry, parentNode);
        }

        public IList<HierarchyNode> GetDescendants(HierarchyEntry hierarchyEntry, HierarchyNode parentNode, bool orderTopDown, bool includeParent)
        {
            return hierarchyEntryDataProvider.GetDescendants(hierarchyEntry, parentNode, orderTopDown, includeParent);
        }

        public IList<HierarchyNode> GetAncestors(HierarchyEntry hierarchyEntry, HierarchyNode node, bool orderTopDown, bool includeChild)
        {
            return hierarchyEntryDataProvider.GetAncestors(hierarchyEntry, node, orderTopDown, includeChild);
        }

        public HierarchyNode GetParent(HierarchyEntry hierarchyEntry, HierarchyNode node)
        {
            return hierarchyEntryDataProvider.GetParent(hierarchyEntry, node);
        }
    }
}