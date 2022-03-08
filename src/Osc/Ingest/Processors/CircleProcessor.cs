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
using OpenSearch.Net;
using OpenSearch.Net.Utf8Json;

namespace Osc
{
	[StringEnum]
	public enum ShapeType
	{
		[EnumMember(Value = "geo_shape")]
		GeoShape,

		[EnumMember(Value = "shape")]
		Shape
	}

	[InterfaceDataContract]
	public interface ICircleProcessor : IProcessor
	{
		/// <summary>
		/// The string-valued field to trim whitespace from.
		/// </summary>
		[DataMember(Name ="field")]
		Field Field { get; set; }

		/// <summary>
		/// The field to assign the polygon shape to, by default field is updated in-place.
		/// </summary>
		[DataMember(Name ="target_field")]
		Field TargetField { get; set; }

		/// <summary>
		/// If true and field does not exist, the processor quietly exits without modifying the document.
		/// </summary>
		[DataMember(Name = "ignore_missing")]
		bool? IgnoreMissing { get; set; }

		/// <summary>
		///  The difference between the resulting inscribed distance from center to side and the circle’s radius
		/// (measured in meters for geo_shape, unit-less for shape)
		/// </summary>
		[DataMember(Name = "error_distance")]
		double? ErrorDistance { get; set; }

		/// <summary>
		/// Which field mapping type is to be used when processing the circle.
		/// </summary>
		[DataMember(Name = "shape_type")]
		ShapeType? ShapeType { get; set; }
	}

	public class CircleProcessor : ProcessorBase, ICircleProcessor
	{
		/// <inheritdoc />
		public Field Field { get; set; }

		/// <inheritdoc />
		public Field TargetField { get; set; }

		/// <inheritdoc />
		public bool? IgnoreMissing { get; set; }

		/// <inheritdoc />
		public double? ErrorDistance { get; set; }

		/// <inheritdoc />
		public ShapeType? ShapeType { get; set; }

		/// <inheritdoc />
		protected override string Name => "circle";
	}

	public class CircleProcessorDescriptor<T>
		: ProcessorDescriptorBase<CircleProcessorDescriptor<T>, ICircleProcessor>, ICircleProcessor
		where T : class
	{
		protected override string Name => "circle";

		Field ICircleProcessor.Field { get; set; }
		Field ICircleProcessor.TargetField { get; set; }
		bool? ICircleProcessor.IgnoreMissing { get; set; }
		double? ICircleProcessor.ErrorDistance { get; set; }
		ShapeType? ICircleProcessor.ShapeType { get; set; }

		/// <inheritdoc cref="ICircleProcessor.Field" />
		public CircleProcessorDescriptor<T> Field(Field field) => Assign(field, (a, v) => a.Field = v);

		/// <inheritdoc cref="ICircleProcessor.Field" />
		public CircleProcessorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) =>
			Assign(objectPath, (a, v) => a.Field = v);

		/// <inheritdoc cref="ICircleProcessor.TargetField" />
		public CircleProcessorDescriptor<T> TargetField(Field field) => Assign(field, (a, v) => a.TargetField = v);

		/// <inheritdoc cref="ICircleProcessor.TargetField" />
		public CircleProcessorDescriptor<T> TargetField<TValue>(Expression<Func<T, TValue>> objectPath) =>
			Assign(objectPath, (a, v) => a.TargetField = v);

		/// <inheritdoc cref="ICircleProcessor.IgnoreMissing">
		public CircleProcessorDescriptor<T> IgnoreMissing(bool? ignoreMissing = true) =>
			Assign(ignoreMissing, (a, v) => a.IgnoreMissing = v);

		/// <inheritdoc cref="ICircleProcessor.IgnoreMissing">
		public CircleProcessorDescriptor<T> ErrorDistance(double? errorDistance) =>
			Assign(errorDistance, (a, v) => a.ErrorDistance = v);

		/// <inheritdoc cref="ICircleProcessor.ShapeType">
		public CircleProcessorDescriptor<T> ShapeType(ShapeType? shapeType) =>
			Assign(shapeType, (a, v) => a.ShapeType = v);
	}
}
