import { composeApiUri, handleHttpResponse } from "@/api/httpHelpers";

export async function getSecurityInfo() {
  const requestOptions = {
    method: "GET",
  };
  try {
    const response = await fetch(
      composeApiUri(`security/info`),
      requestOptions
    );

    return await handleHttpResponse(response);
  } catch (e) {
    console.error("A network error occurred", e);
  }
}

export async function exchangeCode(code, verifier, redirectUri) {
  const requestOptions = {
    method: "GET",
  };

  try {
    const response = await fetch(
      composeApiUri(
        `security/exchange?code=${code}&verifier=${verifier}&redirectUri=${redirectUri}`
      ),
      requestOptions
    );

    return await response;
  } catch (e) {
    console.error("A network error occurred", e);
  }
}
