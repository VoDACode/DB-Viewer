import { HttpClient } from '@angular/common/http';

export class TableApiService {

  public get url(): string {
    return `/api/${this._url}`
  }

  constructor(protected _url: string, private _http: HttpClient) { }

  getAll() {
    return this._http.get<[]>(`${this.url}`);
  }

  get(id: number) {
    return this._http.get(`${this.url}/${id}`);
  }

  create(data: any) {
    return this._http.post(`${this.url}`, data);
  }

  update(id: number, data: any) {
    return this._http.put(`${this.url}/${id}`, data);
  }

  delete(id: number) {
    return this._http.delete(`${this.url}/${id}`);
  }
}
