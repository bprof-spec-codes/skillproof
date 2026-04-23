import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class MarkdownService {
  apiUrl = `${environment.apiUrl}/Markdown`

  constructor(private http:HttpClient){}

  preView(text: string): Observable<string> {
    return this.http.post(`${this.apiUrl}/PreView`, JSON.stringify(text), {
      headers: { 'Content-Type': 'application/json' },
      responseType: 'text'
    });
  }
}
