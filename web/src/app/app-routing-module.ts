import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Login } from './components/login/login';
import { Register } from './components/register/register';
import { HomePage } from './home-page/home-page';
import { QuestionBankList } from './components/question-bank-list/question-bank-list';
import { QuestionBankForm } from './components/question-bank-form/question-bank-form';
import { QuestionBankDetails } from './components/question-bank-details/question-bank-details';

const routes: Routes = [
  {path:"", redirectTo:"login", pathMatch:"full"},
  {path: "home", component:HomePage},
  {path:"login", component:Login},
  {path:"register", component:Register},
  {path:"question-bank", component:QuestionBankList},
  {path:"question-bank/create", component:QuestionBankForm},
  {path:"question-bank/:id/edit", component:QuestionBankForm},
  {path:"question-bank/:id", component:QuestionBankDetails},
  {path:"**", redirectTo:"login", pathMatch:"full"}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
