import { composeApiUri, handleHttpResponse } from "@/api/httpHelpers";
import { getToken } from "@/api/localStorage";

export async function postImportOrganisations(file, onSuccess, onError) {
  const data = new FormData();
  data.append("bulkimportfile", file, file.upload.filename);

  const token = getToken();

  const requestOptions = {
    method: "POST",
    headers: {
      Authorization: `Bearer ${token}`,
    },
    body: data,
  };
  try {
    const response = await fetch(
      composeApiUri(`import/organisations`),
      requestOptions
    );

    return await handleHttpResponse(response, onSuccess, onError);
  } catch (e) {
    console.error("A network error occurred", e);
  }
}

export async function getImportStatus() {
  const token = getToken();

  const requestOptions = {
    method: "GET",
    headers: {
      Authorization: `Bearer ${token}`,
    },
  };

  try {
    const response = await fetch(
      composeApiUri(`import/organisations`),
      requestOptions
    );

    return await handleHttpResponse(response);
  } catch (e) {
    console.error("A network error occurred", e);
  }
}

export async function getImportFile(id) {
  const token = getToken();

  const requestOptions = {
    method: "GET",
    headers: {
      Authorization: `Bearer ${token}`,
    },
  };

  try {
    const response = await fetch(
      composeApiUri(`import/organisations/${id}/content`),
      requestOptions
    );

    switch (response.status) {
      case 200:
      case 202:
        return await response.blob();
      default:
        return new Promise(() => null);
    }
  } catch (e) {
    console.error("A network error occurred", e);
  }
}
