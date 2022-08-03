import { Role, RoleMapping } from "./role.model";

export class User {
  roles: Array<Role>;

  constructor(
    public userName: string,
    roles: Array<string>,
    public ovoNumbers: Array<string>,
    public organisationIds: Array<string>,
    public bodyIds: Array<string>
  ) {
    this.roles = User.toRoles(roles);
  }

  static toRoles(externalRoles): Array<Role> {
    return externalRoles.map((role) => RoleMapping.map(role));
  }
}
