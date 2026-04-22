import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { Header } from './header/header';
import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { Login } from './components/login/login';
import { Register } from './components/register/register';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { HomePage } from './home-page/home-page';
import { errorInterceptor } from './interceptors/error-interceptor';
import { Modal } from './components/modal/modal';
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
import { QuestionTrueFalse } from './components/question-true-false/question-true-false';
import { QuestionMultipleChoice } from './components/question-multiple-choice/question-multiple-choice';

@NgModule({
  declarations: [
    App,
    Login,
    Register,
    Header,
    HomePage,
    Modal,
    QuestionBankList,
    QuestionBankForm,
    QuestionBankDetails,
    EditProfile,
    ProfileView,
    Jobupload,
    JobEdit,
    AssessmentCreate,
    JobDetail,
    TestTake,
    QuestionTrueFalse,
    QuestionMultipleChoice,
  ],
  imports: [BrowserModule, AppRoutingModule, ReactiveFormsModule, FormsModule],
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(withInterceptors([errorInterceptor])),
  ],
  bootstrap: [App],
})
export class AppModule {}
