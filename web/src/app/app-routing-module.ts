import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Login } from './components/login/login';
import { Register } from './components/register/register';
import { HomePage } from './home-page/home-page';

const routes: Routes = [
  {path:"", redirectTo:"homePage", pathMatch:"full"},
  {path:"login", component:Login},
  {path:"register", component:Register},
  {path:"homePage", component:HomePage},
  {path:"**", redirectTo:"homePage", pathMatch:"full"}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
