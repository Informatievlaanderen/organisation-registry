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

using System.Collections.Generic;
using OpenSearch.Net;

namespace Osc
{
	public class StringStatsAggregate : MetricAggregateBase
	{
		/// <summary>
		/// The average length computed over all terms.
		/// </summary>
		public double AverageLength { get; set; }

		/// <summary>
		/// The number of non-empty fields counted.
		/// </summary>
		public long Count { get; set; }

		/// <summary>
		/// The length of the longest term.
		/// </summary>
		public int MaxLength { get; set; }

		/// <summary>
		/// The length of the shortest term.
		/// </summary>
		public int MinLength { get; set; }

		/// <summary>
		/// The Shannon Entropy value computed over all terms collected by the aggregation.
		/// Shannon entropy quantifies the amount of information contained in the field.
		/// It is a very useful metric for measuring a wide range of properties of a data set, such as diversity, similarity, randomness etc.
		/// </summary>
		public double Entropy { get; set; }

		/// <summary>
		/// The probability of each character appearing in all terms.
		/// </summary>
		public IReadOnlyDictionary<string, double> Distribution { get; set; } = EmptyReadOnly<string, double>.Dictionary;
	}
}
