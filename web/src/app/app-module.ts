import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { Header } from './header/header';
import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { Login } from './components/login/login';
import { Register } from './components/register/register';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {
  HttpClient,
  HttpClientModule,
  provideHttpClient,
  withInterceptors,
} from '@angular/common/http';
import { Header } from './header/header';
import { HomePage } from './home-page/home-page';
import { errorInterceptor } from './interceptors/error-interceptor';
import { Modal } from './components/modal/modal';

@NgModule({
  declarations: [App, Login, Register, Header, HomePage, Modal],
  imports: [BrowserModule, AppRoutingModule, ReactiveFormsModule, HttpClientModule, FormsModule],
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(withInterceptors([errorInterceptor])),
  ],
  bootstrap: [App],
})
export class AppModule {}
