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

namespace OpenSearch.Net
{
	public class PipelineException : Exception
	{
		public PipelineException(PipelineFailure failure)
			: base(GetMessage(failure)) => FailureReason = failure;

		public PipelineException(PipelineFailure failure, Exception innerException)
			: base(GetMessage(failure), innerException) => FailureReason = failure;

		public IApiCallDetails ApiCall { get; internal set; }
		public PipelineFailure FailureReason { get; }

		public bool Recoverable =>
			FailureReason == PipelineFailure.BadRequest
			|| FailureReason == PipelineFailure.BadResponse
			|| FailureReason == PipelineFailure.PingFailure;

		public IOpenSearchResponse Response { get; internal set; }

		private static string GetMessage(PipelineFailure failure)
		{
			switch (failure)
			{
				case PipelineFailure.BadRequest: return "An error occurred trying to write the request data to the specified node.";
				case PipelineFailure.BadResponse: return "An error occurred trying to read the response from the specified node.";
				case PipelineFailure.BadAuthentication:
					return "Could not authenticate with the specified node. Try verifying your credentials or check your Shield configuration.";
				case PipelineFailure.PingFailure: return "Failed to ping the specified node.";
				case PipelineFailure.SniffFailure: return "Failed sniffing cluster state.";
				case PipelineFailure.CouldNotStartSniffOnStartup: return "Failed sniffing cluster state upon client startup.";
				case PipelineFailure.MaxTimeoutReached: return "Maximum timeout was reached.";
				case PipelineFailure.MaxRetriesReached: return "The call was retried the configured maximum amount of times";
				case PipelineFailure.NoNodesAttempted:
					return "No nodes were attempted, this can happen when a node predicate does not match any nodes";
				case PipelineFailure.Unexpected:
				default:
					return "An unexpected error occurred. Try checking the original exception for more information.";
			}
		}
	}
}
