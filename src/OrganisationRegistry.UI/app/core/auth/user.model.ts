import { Role } from './role.model';

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
    const roles = new Array<Role>();
    for (let role of externalRoles) {
      switch (role) {
        case 'algemeenBeheerder':
          roles.push(Role.AlgemeenBeheerder);
          break;

        case 'vlimpersBeheerder':
          roles.push(Role.VlimpersBeheerder);
          break;

        case 'orgaanBeheerder':
          roles.push(Role.OrgaanBeheerder);
          break;

        case 'decentraalBeheerder':
          roles.push(Role.DecentraalBeheerder);
          break;

        case 'developer':
          roles.push(Role.Developer);
          break;

        default:
          break;
      }
    }
    return roles;
  }
}
