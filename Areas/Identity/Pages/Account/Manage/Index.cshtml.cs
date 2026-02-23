// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using _420_15D_FX_H26_TP1.Data;
using _420_15D_FX_H26_TP1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace _420_15D_FX_H26_TP1.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<Utilisateur> _userManager;
        private readonly SignInManager<Utilisateur> _signInManager;
        private readonly ApplicationDbContext _context;

        public IndexModel(
            UserManager<Utilisateur> userManager,
            SignInManager<Utilisateur> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        //[TempData]
        //public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public Utilisateur user { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        //public class InputModel
        //{
        //    /// <summary>
        //    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        //    ///     directly from your code. This API may change or be removed in future releases.
        //    /// </summary>
        //    [Phone]
        //    [Display(Name = "Phone number")]
        //    public string PhoneNumber { get; set; }
        //}

        //private async Task LoadAsync(Utilisateur user)
        //{
        //    var userName = await _userManager.GetUserNameAsync(user);
        //    var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

        //    Username = userName;

        //    Input = new InputModel
        //    {
        //        PhoneNumber = phoneNumber
        //    };
        //}

        public async Task<IActionResult> OnGetAsync()
        {
            user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            Username = user.UserName;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid)
            {
                return Page();
            }
            if(Username == null)
            {
                ModelState.AddModelError("Username", "Le nom d'utilisateur ne pourrait être null");
            }
            if (_context.Users.Any(e => e.UserName == Username && e.Email !=user.Email))
            {
                ModelState.AddModelError("Username", "Ce nom d'utilisateur existe déjà");
                return Page();
            }
            Utilisateur userEdit = await _userManager.FindByEmailAsync(user.Email);
            userEdit.Adresse = user.Adresse;
            userEdit.Nom=user.Nom;
            userEdit.Prenom=user.Prenom;
            userEdit.Email=user.Email;
            userEdit.Ville=user.Ville;
            userEdit.codePostal = user.codePostal;
            if (Username != user.UserName)
            {
                var result1 = await _userManager.SetUserNameAsync(userEdit, Username);

                if (!result1.Succeeded)
                {
                    ModelState.AddModelError("Username", $"Erreur lors de la modification du nom d'utilisateur{user.UserName}");
                    return Page();
                }

                // Important! Met à jour les informations d'authentification
                await _signInManager.RefreshSignInAsync(userEdit);
            }
            var result = await _userManager.UpdateAsync(userEdit);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Erreur lors de la modification des informations");
                return Page();
            }
            TempData["SuccessMessage"] = "L'utilisateur est bien modifié";
            return RedirectToPage("/Index");
        }
    }
}
