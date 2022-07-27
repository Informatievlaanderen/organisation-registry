import { ICrudItem } from "core/crud";

export class ContactType implements ICrudItem<ContactType> {
  constructor(
    public id: string = "",
    public name: string = "",
    public regex: string = "",
    public example: string = ""
  ) {}
}
