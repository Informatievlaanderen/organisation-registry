import { composeApiUri, handleHttpResponse } from "@/api/httpHelpers";
import { getToken } from "@/api/localStorage";
import { useAlertStore } from "@/stores/alert";
import alerts from "@/alerts/alerts";

export async function postImportOrganisationCreations({
  file,
  onSuccess,
  onError,
} = {}) {
  return await postFileToUrl(
    file,
    `imports/organisation-creations`,
    onSuccess,
    onError
  );
}

export async function postImportOrganisationTerminations({
  file,
  onSuccess,
  onError,
} = {}) {
  return await postFileToUrl(
    file,
    `imports/organisation-terminations`,
    onSuccess,
    onError
  );
}

export async function getImportStatuses() {
  const token = getToken();

  const requestOptions = {
    method: "GET",
    headers: {
      Authorization: `Bearer ${token}`,
    },
  };

  try {
    const response = await fetch(composeApiUri(`imports`), requestOptions);

    return await handleHttpResponse(response);
  } catch (e) {
    const alertStore = useAlertStore();
    alertStore.setAlert(alerts.serverError);
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
      composeApiUri(`imports/${id}/content`),
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

async function postFileToUrl(file, url, onSuccess, onError) {
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
    const response = await fetch(composeApiUri(url), requestOptions);

    return await handleHttpResponse(response, onSuccess, onError);
  } catch (e) {
    console.error("A network error occurred", e);
  }
}
