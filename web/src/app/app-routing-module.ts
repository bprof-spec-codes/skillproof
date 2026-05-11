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
import { Jobupload } from './components/jobupload/jobupload';
import { JobEdit } from './components/job-edit/job-edit';
import { AssessmentCreate } from './components/assessment-create/assessment-create';
import { JobDetail } from './components/job-detail/job-detail';
import { TestTake } from './components/test-take/test-take';
import { CompanyHome } from './components/company-home/company-home';
import { MyJobs } from './components/my-jobs/my-jobs';
import { ReviewUser } from './components/review-user/review-user';
import { ManualFeedback } from './components/manual-feedback/manual-feedback';

const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'home', component: HomePage },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'question-bank', component: QuestionBankList },
  { path: 'question-bank/create', component: QuestionBankForm },
  { path: 'question-bank/:id/edit', component: QuestionBankForm },
  { path: 'question-bank/:id', component: QuestionBankDetails },
  { path: 'editProfile', component: EditProfile },
  { path: 'viewProfile', component: ProfileView },
  { path: 'job-upload', component: Jobupload },
  { path: 'editJob/:id', component: JobEdit },
  { path: 'job/:id', component: JobDetail },
  { path: 'job/:id/test', component: TestTake },
  { path: 'assessments/create', component: AssessmentCreate },
  { path: 'company', component: CompanyHome },
  { path: 'myJobs', component: MyJobs },
  { path: 'reviewUser/:id', component: ReviewUser },
  { path: 'manualFeedback/:id', component: ManualFeedback },
  { path: '**', redirectTo: 'home', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
