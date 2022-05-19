import { composeApiUri, handleHttpResponse } from "@/api/httpHelpers";

export async function getSecurityInfo() {
  const requestOptions = {
    method: "GET",
  };
  const response = await fetch(composeApiUri(`security/info`), requestOptions);

  return await handleHttpResponse(response);
}

export async function exchangeCode(code, verifier, redirectUri) {
  const requestOptions = {
    method: "GET",
  };
  return await fetch(
    composeApiUri(
      `security/exchange?code=${code}&verifier=${verifier}&redirectUri=${redirectUri}`
    ),
    requestOptions
  );
}
