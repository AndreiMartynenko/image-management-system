package uk.ac.kingston.ci7250.ims.web.ui;

import jakarta.validation.Valid;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.validation.BindingResult;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.ModelAttribute;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import uk.ac.kingston.ci7250.ims.service.StaffService;
import uk.ac.kingston.ci7250.ims.web.dto.CreateStaffRequest;

@Controller
@RequestMapping("/ui/admin/staff")
public class UiAdminStaffController {
    private final StaffService staffService;

    public UiAdminStaffController(StaffService staffService) {
        this.staffService = staffService;
    }

    @GetMapping
    public String list(Model model) {
        model.addAttribute("staff", staffService.list());
        if (!model.containsAttribute("createStaffRequest")) {
            model.addAttribute("createStaffRequest", new CreateStaffRequest());
        }
        return "ui/admin-staff";
    }

    @PostMapping
    public String create(@Valid @ModelAttribute CreateStaffRequest createStaffRequest,
                         BindingResult bindingResult,
                         Model model) {
        if (bindingResult.hasErrors()) {
            model.addAttribute("staff", staffService.list());
            return "ui/admin-staff";
        }

        staffService.create(createStaffRequest.getName(), createStaffRequest.getRole());
        return "redirect:/ui/admin/staff";
    }
}
