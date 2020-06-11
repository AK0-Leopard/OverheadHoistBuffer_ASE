// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="FlowRelation.cs" company="">
//     Copyright ©  2014
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.bcf.Data.FlowRule;

namespace com.mirle.ibg3k0.sc.Data.VO
{
    /// <summary>
    /// Class FlowRelation.
    /// </summary>
    public class FlowRelation
    {
        /// <summary>
        /// The flow_rule
        /// </summary>
        private IFlowRule flow_rule = null;
        /// <summary>
        /// Gets the flow_ rule.
        /// </summary>
        /// <value>The flow_ rule.</value>
        public IFlowRule Flow_Rule { get { return flow_rule; } }

        /// <summary>
        /// The upstream_id
        /// </summary>
        private string upstream_id = null;
        /// <summary>
        /// Gets the upstream_ identifier.
        /// </summary>
        /// <value>The upstream_ identifier.</value>
        public string Upstream_ID { get { return upstream_id; } }

        /// <summary>
        /// The flow relation items
        /// </summary>
        private List<AFLOW_REL> flowRelationItems = new List<AFLOW_REL>();
        /// <summary>
        /// Gets the flow relation items.
        /// </summary>
        /// <value>The flow relation items.</value>
        public List<AFLOW_REL> FlowRelationItems { get { return flowRelationItems; } }

        /// <summary>
        /// The is donot care flow rule
        /// </summary>
        private Boolean isDonotCareFlowRule = false;
        /// <summary>
        /// Gets the is donot care flow rule.
        /// </summary>
        /// <value>The is donot care flow rule.</value>
        public Boolean IsDonotCareFlowRule { get { return isDonotCareFlowRule; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlowRelation"/> class.
        /// </summary>
        /// <param name="upstream_id">The upstream_id.</param>
        /// <param name="flow_rule">The flow_rule.</param>
        /// <param name="flowRelationItems">The flow relation items.</param>
        /// <param name="isDonotCareFlowRule">The is donot care flow rule.</param>
        public FlowRelation(string upstream_id, IFlowRule flow_rule, List<AFLOW_REL> flowRelationItems,
            Boolean isDonotCareFlowRule) 
        {
            this.upstream_id = upstream_id;
            this.flow_rule = flow_rule;
            this.flowRelationItems = flowRelationItems;
            this.isDonotCareFlowRule = isDonotCareFlowRule;
        }

        /// <summary>
        /// Gets the downstream identifier list.
        /// </summary>
        /// <returns>List&lt;String&gt;.</returns>
        public List<String> getDownstreamIDList() 
        {
            List<string> idList = new List<string>();
            if (flowRelationItems == null) 
            {
                return idList;
            }
            foreach (AFLOW_REL item in flowRelationItems) 
            {
                idList.Add(item.DOWNSTREAM_ID.Trim());
            }
            return idList;
        }

    }
}
