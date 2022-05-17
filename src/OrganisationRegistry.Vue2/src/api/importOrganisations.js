import { composeApiUri, handleHttpResponse } from "@/api/httpHelpers";
import { getToken } from "@/api/security";

export async function postImportOrganisations(file) {
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
  return await fetch(
    composeApiUri(`import/organisations`),
    requestOptions
  ).then((response) => handleHttpResponse(response));
}

export function getImportStatus() {}
