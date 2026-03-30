import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Login } from './components/login/login';
import { Register } from './components/register/register';
import { HomePage } from './home-page/home-page';
import { QuestionBankList } from './components/question-bank-list/question-bank-list';
import { QuestionBankForm } from './components/question-bank-form/question-bank-form';
import { QuestionBankDetails } from './components/question-bank-details/question-bank-details';
import { EditProfile } from './components/edit-profile/edit-profile';
import { ProfileView } from './components/profile-view/profile-view';

const routes: Routes = [
  {path:"", redirectTo:"home", pathMatch:"full"},
  {path:"homePage", component: HomePage},
  {path:"login", component:Login},
  {path:"register", component:Register},
  {path:"question-bank", component:QuestionBankList},
  {path:"question-bank/create", component:QuestionBankForm},
  {path:"question-bank/:id/edit", component:QuestionBankForm},
  {path:"question-bank/:id", component:QuestionBankDetails},
  {path:"editProfile", component:EditProfile},
  {path:"viewProfile", component:ProfileView},
  {path:"**", redirectTo:"home", pathMatch:"full"}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
