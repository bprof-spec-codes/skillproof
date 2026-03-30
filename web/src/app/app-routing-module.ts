import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Login } from './components/login/login';
import { Register } from './components/register/register';
import { HomePage } from './home-page/home-page';
import { ProfileView } from './components/profile-view/profile-view';

const routes: Routes = [
  {path:"", redirectTo:"homePage", pathMatch:"full"},
  {path:"login", component:Login},
  {path:"register", component:Register},
  {path:"homePage", component:HomePage},
  {path:"profileView", component:ProfileView},
  {path:"**", redirectTo:"homePage", pathMatch:"full"}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
