import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ResultModel } from '../models/result.model';
import { ApiConstants } from '../constants/api.constants';

@Injectable({
  providedIn: 'root'
})
export class HttpService {

  constructor(private readonly http: HttpClient) { }

  post<T>(endpoint: string, body: any, callback: (res: ResultModel<T>) => void, errorCallback?: (err: HttpErrorResponse) => void) {

    return this.http.post<ResultModel<T>>(`${ApiConstants.BASE_URL}/${endpoint}`, body, {
      headers: {
        'Content-Type': 'application/json'
      }
    }).subscribe({
      next: (response) => {
        callback(response);
      },
      error: (error: HttpErrorResponse) => {

        if (errorCallback) {
          errorCallback(error);
        }
      }
    });
  }
}
