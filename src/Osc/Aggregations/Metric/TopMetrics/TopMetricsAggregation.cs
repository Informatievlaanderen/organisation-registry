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
using System.Runtime.Serialization;
using OpenSearch.Net.Utf8Json;

namespace Osc
{
	[InterfaceDataContract]
	[ReadAs(typeof(TopMetricsAggregation))]
	public interface ITopMetricsAggregation : IMetricAggregation
	{
		/// <summary>
		/// Metrics selects the fields of the "top" document to return. You can request a single metric or multiple metrics.
		/// </summary>
		[DataMember(Name ="metrics")]
		IList<ITopMetricsValue> Metrics { get; set; }

		/// <summary>
		/// Return the top few documents worth of metrics using this parameter.
		/// </summary>
		[DataMember(Name ="size")]
		int? Size { get; set; }

		/// <summary>
		/// The sort field in the metric request functions exactly the same as the sort field in the search request except:
		/// * It can’t be used on binary, ip, keyword, or text fields.
		/// * It only supports a single sort value so which document wins ties is not specified.
		/// </summary>
		[DataMember(Name ="sort")]
		IList<ISort> Sort { get; set; }
	}

	public class TopMetricsAggregation : MetricAggregationBase, ITopMetricsAggregation
	{
		internal TopMetricsAggregation() { }

		public TopMetricsAggregation(string name) : base(name, null) { }

		/// <inheritdoc cref="ITopMetricsAggregation.Metrics" />
		public IList<ITopMetricsValue> Metrics { get; set; }

		/// <inheritdoc cref="ITopMetricsAggregation.Size" />
		public int? Size { get; set; }

		/// <inheritdoc cref="ITopMetricsAggregation.Sort" />
		public IList<ISort> Sort { get; set; }

		internal override void WrapInContainer(AggregationContainer c) => c.TopMetrics = this;
	}

	public class TopMetricsAggregationDescriptor<T>
		: MetricAggregationDescriptorBase<TopMetricsAggregationDescriptor<T>, ITopMetricsAggregation, T>, ITopMetricsAggregation where T : class
	{
		int? ITopMetricsAggregation.Size { get; set; }

		IList<ISort> ITopMetricsAggregation.Sort { get; set; }

		IList<ITopMetricsValue> ITopMetricsAggregation.Metrics { get; set; }

		/// <inheritdoc cref="ITopMetricsAggregation.Size" />
		public TopMetricsAggregationDescriptor<T> Size(int? size) => Assign(size, (a, v) =>
			a.Size = v);

		/// <inheritdoc cref="ITopMetricsAggregation.Sort" />
		public TopMetricsAggregationDescriptor<T> Sort(Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortSelector) =>
			Assign(sortSelector, (a, v) =>
				a.Sort = v?.Invoke(new SortDescriptor<T>())?.Value);

		/// <inheritdoc cref="ITopMetricsAggregation.Metrics" />
		public TopMetricsAggregationDescriptor<T> Metrics(Func<TopMetricsValuesDescriptor<T>, IPromise<IList<ITopMetricsValue>>> TopMetricsValueSelector) =>
			Assign(TopMetricsValueSelector, (a, v) =>
				a.Metrics = v?.Invoke(new TopMetricsValuesDescriptor<T>())?.Value);
	}
}
