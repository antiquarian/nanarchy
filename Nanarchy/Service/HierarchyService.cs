using System.Collections.Generic;
using Nanarchy.Core;
using Nanarchy.Core.Interfaces;

namespace Nanarchy.Service
{
    public class HierarchyService : IHierarchyService
    {
        private readonly IHierarchyDataProvider _hierarchyDataProvider;
        private readonly HierarchyEntry _hierarchy;
        private readonly IHierarchyEntryRepository _hierarchyRepository;
        private readonly ITargetEntryRepository _targetRepository;

        public HierarchyService(
            IHierarchyDataProvider hierarchyDataProvider, 
            HierarchyEntry hierarchy,
            IHierarchyEntryRepository  hierarchyRepository,
            ITargetEntryRepository targetRepository)
        {
            _hierarchyDataProvider = hierarchyDataProvider;
            _hierarchy = hierarchy;
            _hierarchyRepository = hierarchyRepository;
            _targetRepository = targetRepository;
        }

        #region Initialization Methods - used on startup
        public bool InitializeDatabase(HierarchyEntry hierarchy, List<TargetEntry> nodeTargets)
        {
            var check = VerifyManagementTables();
            if (!check) return false;

            _hierarchyRepository.Update(hierarchy);
            foreach (var nodeTarget in nodeTargets)
            {
                _targetRepository.Update(nodeTarget);
            }
            return true;
        }
        private bool VerifyManagementTables()
        {
            if (!_hierarchyRepository.TableExists())
            {
                _hierarchyRepository.Initialize();
                if (!_hierarchyRepository.TableExists()) return false;
            }

            if (!_targetRepository.TableExists())
            {
                _targetRepository.Initialize();
                if (!_targetRepository.TableExists()) return false;
            }
            return true;
        }

        #endregion


        public HierarchyNode InitializeHierarchy(ITarget rootTarget)
        {
            var rootNode = new HierarchyNode
            {
                LeftId = 1,
                RightId = 2,
                TargetId = rootTarget == null ? 0 : rootTarget.Id 
            };

            // initialize the target table for the node
            _hierarchyDataProvider.Add(_hierarchy, rootNode);

            return rootNode;
        }

        public HierarchyNode GetRootNode()
        {
            return _hierarchyDataProvider.GetRootNode(_hierarchy);
        }

        public HierarchyNode InsertNode(HierarchyNode parentNode, ITarget childTarget)
        {
            var childNode = new HierarchyNode { TargetId = childTarget.Id };
            childNode = PrepareForInsertNode(parentNode, childNode);
            _hierarchyDataProvider.Add(_hierarchy, childNode);
            return childNode;
        }

        public HierarchyNode PrepareForInsertNode(HierarchyNode parentNode, HierarchyNode childNode)
        {
            childNode.LeftId = parentNode.RightId;
            _hierarchyDataProvider.PrepareForInsertNode(_hierarchy, parentNode);
            childNode.RightId = childNode.LeftId + 1;
            return childNode;
        }

        public void DeleteNode(HierarchyNode node)
        {
            _hierarchyDataProvider.Delete(_hierarchy, node);
        }

        public IList<HierarchyNode> GetChildren(HierarchyNode parentNode)
        {
            return _hierarchyDataProvider.GetChildren(_hierarchy, parentNode);
        }

        public IList<HierarchyNode> GetDescendants(HierarchyNode parentNode, bool orderTopDown, bool includeParent)
        {
            return _hierarchyDataProvider.GetDescendants(_hierarchy, parentNode, orderTopDown, includeParent);
        }

        public IList<HierarchyNode> GetAncestors(HierarchyNode node, bool orderTopDown, bool includeChild)
        {
            return _hierarchyDataProvider.GetAncestors(_hierarchy, node, orderTopDown, includeChild);
        }

        public HierarchyNode GetParent(HierarchyNode node)
        {
            return _hierarchyDataProvider.GetParent(_hierarchy, node);
        }
    }
}