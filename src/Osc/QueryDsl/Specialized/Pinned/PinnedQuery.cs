/* SPDX-License-Identifier: Apache-2.0
*
* The OpenSearch Contributors require contributions made to
* this file be licensed under the Apache-2.0 license or a
* compatible open source license.
*
* Modifications Copyright OpenSearch Contributors. See
* GitHub history for details.
*
*  Licensed to Elasticsearch B.V. under one or more contributor
*  license agreements. See the NOTICE file distributed with
*  this work for additional information regarding copyright
*  ownership. Elasticsearch B.V. licenses this file to you under
*  the Apache License, Version 2.0 (the "License"); you may
*  not use this file except in compliance with the License.
*  You may obtain a copy of the License at
*
* 	http://www.apache.org/licenses/LICENSE-2.0
*
*  Unless required by applicable law or agreed to in writing,
*  software distributed under the License is distributed on an
*  "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
*  KIND, either express or implied.  See the License for the
*  specific language governing permissions and limitations
*  under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using OpenSearch.Net.Utf8Json;

namespace Osc
{
	/// <summary>
	/// Promotes selected documents to rank higher than those matching a given query. This feature is typically used to
	/// guide searchers to curated documents that are promoted over and above any "organic" matches for a search.
	/// The promoted or "pinned" documents are identified using the document IDs stored in the _id field.
	/// </summary>
	[InterfaceDataContract]
	[ReadAs(typeof(PinnedQuery))]
	public interface IPinnedQuery : IQuery
	{
		/// <summary>
		/// An array of document IDs listed in the order they are to appear in results.
		/// </summary>
		[DataMember(Name = "ids")]
		IEnumerable<Id> Ids { get; set; }

		/// <summary>
		/// Any choice of query used to rank documents which will be ranked below the "pinned" document ids.
		/// </summary>
		[DataMember(Name = "organic")]
		QueryContainer Organic { get; set; }
	}

	/// <inheritdoc cref="IPinnedQuery"/>
	public class PinnedQuery : QueryBase, IPinnedQuery
	{
		/// <inheritdoc />
		public IEnumerable<Id> Ids { get; set; }

		/// <inheritdoc />
		public QueryContainer Organic { get; set; }

		protected override bool Conditionless => IsConditionless(this);

		internal override void InternalWrapInContainer(IQueryContainer c) => c.Pinned = this;

		internal static bool IsConditionless(IPinnedQuery q) => !q.Ids.HasAny() && q.Organic.IsConditionless();
	}

	/// <inheritdoc cref="IPinnedQuery"/>
	public class PinnedQueryDescriptor<T>
		: QueryDescriptorBase<PinnedQueryDescriptor<T>, IPinnedQuery>
			, IPinnedQuery
		where T : class
	{
		protected override bool Conditionless => PinnedQuery.IsConditionless(this);
		IEnumerable<Id> IPinnedQuery.Ids { get; set; }
		QueryContainer IPinnedQuery.Organic { get; set; }

		/// <inheritdoc cref="IPinnedQuery.Ids"/>
		public PinnedQueryDescriptor<T> Ids(params Id[] ids) =>
			Assign(ids, (a, v) => a.Ids = v);

		/// <inheritdoc cref="IPinnedQuery.Ids"/>
		public PinnedQueryDescriptor<T> Ids(IEnumerable<Id> ids) =>
			Assign(ids, (a, v) => a.Ids = v);

		/// <inheritdoc cref="IPinnedQuery.Ids"/>
		public PinnedQueryDescriptor<T> Ids(IEnumerable<string> ids) =>
			Assign(ids?.Select(i => (Id)i), (a, v) => a.Ids = v);

		/// <inheritdoc cref="IPinnedQuery.Ids"/>
		public PinnedQueryDescriptor<T> Ids(IEnumerable<long> ids) =>
			Assign(ids?.Select(i => (Id)i), (a, v) => a.Ids = v);

		/// <inheritdoc cref="IPinnedQuery.Ids"/>
		public PinnedQueryDescriptor<T> Ids(IEnumerable<Guid> ids) =>
			Assign(ids?.Select(i => (Id)i), (a, v) => a.Ids = v);

		/// <inheritdoc cref="IPinnedQuery.Organic"/>
		public PinnedQueryDescriptor<T> Organic(Func<QueryContainerDescriptor<T>, QueryContainer> selector) =>
			Assign(selector, (a, v) => a.Organic = v?.Invoke(new QueryContainerDescriptor<T>()));
	}
}
