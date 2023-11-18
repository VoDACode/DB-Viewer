import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SqlParam } from '../models/SqlParam';
import { catchError, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SqlApiService {

  constructor(private http: HttpClient) { }

  public getFunctionNames() {
    return this.http.get<string[]>('/api/sql/func');
  }

  public getProcedureNames() {
    return this.http.get<string[]>('/api/sql/proc');
  }

  public getParams(name: string) {
    return this.http.get<SqlParam[]>(`/api/sql/params/${name}`);
  }

  public runProcedure(name: string, params: SqlParam[]) {
    var url = `/api/sql/proc/${name}/call?`;
    for (var param of params) {
      if (!param.value)
        continue;
      url += `${param.name}=${param.value}&`;
    }
    url = url.slice(0, -1);
    return this.http.get(url, {
      responseType: "text"
    }).pipe(
      catchError(this.handleError)
    );
  }

  public runFunction(name: string, params: SqlParam[]) {
    var url = `/api/sql/func/${name}/call?`;
    for (var param of params) {
      if (!param.value)
        continue;
      url += `${param.name}=${param.value}&`;
    }
    url = url.slice(0, -1);
    return this.http.get(url, {
      responseType: "text"
    }).pipe(
      catchError(this.handleError)
    );
  }

  private handleError(error: HttpErrorResponse) {
    if (error.status === 0) {
      alert('An error occurred:' + error.error);
    } else {
      alert(error.error);
    }
    // Return an observable with a user-facing error message.
    return throwError(() => new Error('Something bad happened; please try again later.'));
  }
}
