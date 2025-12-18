package uk.ac.kingston.ci7250.ims.web;

import jakarta.validation.Valid;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;
import uk.ac.kingston.ci7250.ims.domain.Staff;
import uk.ac.kingston.ci7250.ims.service.StaffService;
import uk.ac.kingston.ci7250.ims.web.dto.CreateStaffRequest;

import java.util.List;

@RestController
@RequestMapping("/api/admin/staff")
public class StaffController {
    private final StaffService staffService;

    public StaffController(StaffService staffService) {
        this.staffService = staffService;
    }

    @PostMapping
    @ResponseStatus(HttpStatus.CREATED)
    public Staff create(@Valid @RequestBody CreateStaffRequest req) {
        return staffService.create(req.getName(), req.getRole());
    }

    @GetMapping
    public List<Staff> list() {
        return staffService.list();
    }

    @GetMapping("/{id}")
    public Staff get(@PathVariable long id) {
        return staffService.get(id);
    }

    @DeleteMapping("/{id}")
    @ResponseStatus(HttpStatus.NO_CONTENT)
    public void delete(@PathVariable long id) {
        staffService.delete(id);
    }
}
