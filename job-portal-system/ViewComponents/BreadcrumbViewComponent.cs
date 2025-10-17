using Microsoft.AspNetCore.Mvc;

namespace job_portal_system.ViewComponents
{
    public class BreadcrumbViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var routeData = ViewContext.RouteData.Values;
            var controller = routeData["controller"]?.ToString() ?? "";
            var action = routeData["action"]?.ToString() ?? "";

            var breadcrumbs = GenerateBreadcrumbs(controller, action);
            return View(breadcrumbs);
        }

        private List<BreadcrumbItem> GenerateBreadcrumbs(string controller, string action)
        {
            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Home", Url = "/", Icon = "bi-house-door" }
            };

            // Controller-specific breadcrumbs
            switch (controller.ToLower())
            {
                case "admin":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Admin", Url = "/Admin/Dashboard", Icon = "bi-shield-check" });
                    AddAdminActionBreadcrumb(breadcrumbs, action);
                    break;

                case "employer":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Employer", Url = "/Employer/Dashboard", Icon = "bi-building" });
                    AddEmployerActionBreadcrumb(breadcrumbs, action);
                    break;

                case "account":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Account", Url = "#", Icon = "bi-person-circle" });
                    AddAccountActionBreadcrumb(breadcrumbs, action);
                    break;

                case "job":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Jobs", Url = "/Job/Index", Icon = "bi-briefcase" });
                    AddJobActionBreadcrumb(breadcrumbs, action);
                    break;

                case "application":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Applications", Url = "/Application/MyApplications", Icon = "bi-file-earmark-text" });
                    AddApplicationActionBreadcrumb(breadcrumbs, action);
                    break;

                case "companies":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Companies", Url = "/Companies/Index", Icon = "bi-buildings" });
                    AddCompaniesActionBreadcrumb(breadcrumbs, action);
                    break;

                case "contact":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Contact", Url = "/Contact/Index", Icon = "bi-envelope" });
                    break;

                case "messages":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Messages", Url = "/Messages/Inbox", Icon = "bi-chat-dots" });
                    AddMessagesActionBreadcrumb(breadcrumbs, action);
                    break;

                case "admincontact":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Admin", Url = "/Admin/Dashboard", Icon = "bi-shield-check" });
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Contact Messages", Url = "/AdminContact/Messages", Icon = "bi-envelope", IsActive = true });
                    break;
            }

            return breadcrumbs;
        }

        private void AddAdminActionBreadcrumb(List<BreadcrumbItem> breadcrumbs, string action)
        {
            switch (action.ToLower())
            {
                case "dashboard":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Dashboard", Icon = "bi-speedometer2", IsActive = true });
                    break;
                case "manageusers":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Manage Users", Icon = "bi-people", IsActive = true });
                    break;
                case "pendingemployers":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Pending Approvals", Icon = "bi-clock-history", IsActive = true });
                    break;
                case "managejobs":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Manage Jobs", Icon = "bi-briefcase", IsActive = true });
                    break;
                case "reports":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Reports", Icon = "bi-bar-chart", IsActive = true });
                    break;
            }
        }

        private void AddEmployerActionBreadcrumb(List<BreadcrumbItem> breadcrumbs, string action)
        {
            switch (action.ToLower())
            {
                case "dashboard":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Dashboard", Icon = "bi-speedometer2", IsActive = true });
                    break;
                case "applicants":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Applicants", Icon = "bi-people", IsActive = true });
                    break;
            }
        }

        private void AddAccountActionBreadcrumb(List<BreadcrumbItem> breadcrumbs, string action)
        {
            switch (action.ToLower())
            {
                case "profile":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Profile", Icon = "bi-person", IsActive = true });
                    break;
                case "security":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Security", Icon = "bi-shield-lock", IsActive = true });
                    break;
                case "login":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Login", Icon = "bi-box-arrow-in-right", IsActive = true });
                    break;
                case "register":
                case "registerjobseeker":
                case "registeremployer":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Register", Icon = "bi-person-plus", IsActive = true });
                    break;
                case "forgotpassword":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Forgot Password", Icon = "bi-key", IsActive = true });
                    break;
            }
        }

        private void AddJobActionBreadcrumb(List<BreadcrumbItem> breadcrumbs, string action)
        {
            switch (action.ToLower())
            {
                case "index":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Browse Jobs", Icon = "bi-search", IsActive = true });
                    break;
                case "details":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Job Details", Icon = "bi-info-circle", IsActive = true });
                    break;
                case "create":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Post Job", Icon = "bi-plus-circle", IsActive = true });
                    break;
                case "managejobs":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "My Jobs", Icon = "bi-list", IsActive = true });
                    break;
                case "edit":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Edit Job", Icon = "bi-pencil", IsActive = true });
                    break;
            }
        }

        private void AddApplicationActionBreadcrumb(List<BreadcrumbItem> breadcrumbs, string action)
        {
            switch (action.ToLower())
            {
                case "myapplications":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "My Applications", Icon = "bi-file-text", IsActive = true });
                    break;
                case "apply":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Apply for Job", Icon = "bi-send", IsActive = true });
                    break;
            }
        }

        private void AddCompaniesActionBreadcrumb(List<BreadcrumbItem> breadcrumbs, string action)
        {
            switch (action.ToLower())
            {
                case "index":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Browse Companies", Icon = "bi-search", IsActive = true });
                    break;
                case "details":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Company Details", Icon = "bi-info-circle", IsActive = true });
                    break;
            }
        }

        private void AddMessagesActionBreadcrumb(List<BreadcrumbItem> breadcrumbs, string action)
        {
            switch (action.ToLower())
            {
                case "inbox":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Inbox", Icon = "bi-inbox", IsActive = true });
                    break;
                case "sent":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Sent", Icon = "bi-send", IsActive = true });
                    break;
                case "compose":
                    breadcrumbs.Add(new BreadcrumbItem { Text = "Compose", Icon = "bi-pencil-square", IsActive = true });
                    break;
            }
        }
    }

    public class BreadcrumbItem
    {
        public string Text { get; set; } = string.Empty;
        public string? Url { get; set; }
        public string Icon { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
