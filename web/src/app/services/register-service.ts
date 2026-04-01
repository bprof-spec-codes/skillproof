import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { RegisterDto } from '../Models/Dtos/User/register-dto';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class RegisterService {
  constructor(private http: HttpClient) {}
  apiUrl: string = environment.apiUrl;

  register(dto: any): Observable<any> {
    if (dto.role === 'Employer') {
      return this.http.post(`${this.apiUrl}/User/RegisterEmployer`, dto);
    } else {
      return this.http.post(`${this.apiUrl}/User/Register`, dto);
    }
  }
}
