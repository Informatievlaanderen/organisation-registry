﻿/**
 * Composes the full API uri based on a fixed route prefix and a relative route.
 * @param {string} route - relative route NOT starting with '/'
 * @returns {string} - The API uri
 */
export function composeApiUri(route) {
  return `${window.organisationRegistryApiEndpoint}/v1/${route}`;
}

/**
 * Handles the incoming response based on its http status code.
 * @param {Response} response - the incoming response
 * @param onSuccess {(response: Response) => {}}
 * @param onError {(response: Response) => {}}
 * @returns {Promise<any | null>} - <p>Returns null when the http status code is not a success code with a response.</p>
 *                                <p>Returns Promise of JSON body when the http status code is a success code with a response (200, 202).</p>
 */
export async function handleHttpResponse(response, onSuccess, onError) {
  switch (response.status) {
    case 200:
    case 202:
      onSuccess && onSuccess(response);
      return await response.json();
    case 400:
      onError && onError(response);
      return new Promise(() => null);
    default:
      onError && onError(response);
      return new Promise(() => null);
  }
}
