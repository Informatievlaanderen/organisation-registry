import { SearchResult } from './search-result.model';

export interface SearchCallback {
  (search: string): SearchResult[];
}
