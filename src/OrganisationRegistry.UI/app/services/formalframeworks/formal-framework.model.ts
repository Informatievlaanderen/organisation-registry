import { ICrudItem } from 'core/crud';

export class FormalFramework implements ICrudItem<FormalFramework> {
  constructor(
    public id: string = '',
    public name: string = '',
    public code: string = '',
    public formalFrameworkCategoryId: string = '',
    public formalFrameworkCategoryName: string = ''
  ) { }
}
