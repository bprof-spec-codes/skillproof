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
import { AdminSkill } from './components/admin-skill/admin-skill';
import { authGuard } from './interceptors/auth-guard';
import { JobSearch } from './components/job-search/job-search';
import { FullJobView } from './components/full-job-view/full-job-view';

const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },

  { path: 'home', component: HomePage},
  
  { path: 'login', component: Login },
  { path: 'register', component: Register },

  { path: 'question-bank', component: QuestionBankList,
    canActivate: [authGuard],
    data: { roles: ['EMPLOYER'] }
  },

  { path: 'question-bank/create', component: QuestionBankForm,
    canActivate: [authGuard],
    data: { roles: ['EMPLOYER'] }
  },

  { path: 'question-bank/:id/edit', component: QuestionBankForm,
    canActivate: [authGuard],
    data: { roles: ['EMPLOYER'] }
  },

  { path: 'question-bank/:id', component: QuestionBankDetails,
    canActivate: [authGuard],
    data: { roles: ['EMPLOYER'] }
  },

  { path: 'editProfile', component: EditProfile, canActivate: [authGuard] },
  { path: 'viewProfile', component: ProfileView, canActivate: [authGuard] },

  { path: 'job-upload', component: Jobupload,
    canActivate: [authGuard],
    data: { roles: ['EMPLOYER'] }
  },

  { path: 'editJob/:id', component: JobEdit,
    canActivate: [authGuard],
    data: { roles: ['EMPLOYER'] }
  },

  { path: 'job/:id', component: JobDetail
  },

  { path: 'job/:id/test', component: TestTake
  },

  { path: 'assessments/create', component: AssessmentCreate,
    canActivate: [authGuard],
    data: { roles: ['EMPLOYER'] }
  },

  { path: 'company', component: CompanyHome,
    canActivate: [authGuard],
    data: { roles: ['EMPLOYER'] }
  },

  { path: 'myJobs', component: MyJobs,
    canActivate: [authGuard],
    data: { roles: ['EMPLOYER'] }
  },

  { path: 'reviewUser/:id', component: ReviewUser,
    canActivate: [authGuard],
    data: { roles: ['EMPLOYER'] }
  },

  { path: 'manualFeedback/:id', component: ManualFeedback,
    canActivate: [authGuard],
    data: { roles: ['EMPLOYER'] }
  },
  
  { path: 'skills', component: AdminSkill},
  
  { path: 'skill/:skillId/test/:assessmentId', component: TestTake,
    canActivate: [authGuard]
   },

  {path: 'search', component: JobSearch},

  { path: 'full-job-view/:id', component: FullJobView },

  { path: '**', redirectTo: 'home', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
