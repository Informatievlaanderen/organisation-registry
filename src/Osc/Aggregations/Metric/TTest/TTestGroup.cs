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
using System.Linq.Expressions;
using System.Runtime.Serialization;
using OpenSearch.Net.Utf8Json;

namespace Osc
{
	/// <summary>
	/// A population for a <see cref="TTestAggregation"/>
	/// </summary>
	[InterfaceDataContract]
	[ReadAs(typeof(TTestPopulation))]
	public interface ITTestPopulation
	{
		/// <summary>
		/// The field to use for the population values. Must be a numeric field.
		/// </summary>
		[DataMember(Name = "field")]
		Field Field { get; set; }

		/// <summary>
		/// A script tp use to calculate population values.
		/// </summary>
		[DataMember(Name = "script")]
		IScript Script { get; set; }

		/// <summary>
		/// A filter to apply to target field to filter population values. Useful
		/// when two populations use the same field for values, to filter the values.
		/// </summary>
		[DataMember(Name = "filter")]
		QueryContainer Filter { get; set; }
	}

	/// <inheritdoc />
	public class TTestPopulation : ITTestPopulation
	{
		/// <inheritdoc />
		public Field Field { get; set; }
		/// <inheritdoc />
		public IScript Script { get; set; }
		/// <inheritdoc />
		public QueryContainer Filter { get; set; }
	}

	/// <inheritdoc cref="ITTestPopulation"/>
	public class TTestPopulationDescriptor<T> : DescriptorBase<TTestPopulationDescriptor<T>, ITTestPopulation>, ITTestPopulation where T : class
	{
		Field ITTestPopulation.Field { get; set; }
		IScript ITTestPopulation.Script { get; set; }
		QueryContainer ITTestPopulation.Filter { get; set; }

		/// <inheritdoc cref="ITTestPopulation.Field"/>
		public TTestPopulationDescriptor<T> Field(Field field) => Assign(field, (a, v) => a.Field = v);

		/// <inheritdoc cref="ITTestPopulation.Field"/>
		public TTestPopulationDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> field) => Assign(field, (a, v) => a.Field = v);

		/// <inheritdoc cref="ITTestPopulation.Script"/>
		public TTestPopulationDescriptor<T> Script(string script) => Assign((InlineScript)script, (a, v) => a.Script = v);

		/// <inheritdoc cref="ITTestPopulation.Script"/>
		public TTestPopulationDescriptor<T> Script(Func<ScriptDescriptor, IScript> scriptSelector) =>
			Assign(scriptSelector, (a, v) => a.Script = v?.Invoke(new ScriptDescriptor()));

		/// <inheritdoc cref="ITTestPopulation.Filter"/>
		public TTestPopulationDescriptor<T> Filter(Func<QueryContainerDescriptor<T>, QueryContainer> filter) =>
			Assign(filter, (a, v) => a.Filter = v?.Invoke(new QueryContainerDescriptor<T>()));
	}
}
