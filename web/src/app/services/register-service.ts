import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { RegisterDto } from '../Models/Dtos/User/register-dto';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class RegisterService {

  constructor(private http:HttpClient){}

  register(dto:RegisterDto):Observable<any>{
    return this.http.post(environment.apiUrls.register, dto)
  }

}
