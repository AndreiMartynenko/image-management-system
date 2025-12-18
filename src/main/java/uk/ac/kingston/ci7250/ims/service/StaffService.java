package uk.ac.kingston.ci7250.ims.service;

import org.springframework.stereotype.Service;
import uk.ac.kingston.ci7250.ims.domain.Staff;
import uk.ac.kingston.ci7250.ims.repo.StaffRepository;

import java.util.List;

@Service
public class StaffService {
    private final StaffRepository staffRepository;

    public StaffService(StaffRepository staffRepository) {
        this.staffRepository = staffRepository;
    }

    public Staff create(String name, String role) {
        if (name == null || name.isBlank()) {
            throw new IllegalArgumentException("Staff name is required");
        }
        if (role == null || role.isBlank()) {
            throw new IllegalArgumentException("Staff role is required");
        }
        return staffRepository.create(name, role);
    }

    public Staff get(long id) {
        return staffRepository.require(id);
    }

    public List<Staff> list() {
        return staffRepository.findAllSortedById();
    }

    public void delete(long id) {
        staffRepository.delete(id);
    }
}
