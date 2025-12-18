package uk.ac.kingston.ci7250.ims.repo;

import org.springframework.stereotype.Repository;
import uk.ac.kingston.ci7250.ims.domain.Staff;

import java.util.ArrayList;
import java.util.Comparator;
import java.util.List;
import java.util.Map;
import java.util.NoSuchElementException;
import java.util.Optional;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.atomic.AtomicLong;

@Repository
public class StaffRepository {
    private final AtomicLong idSeq = new AtomicLong(2000);
    private final Map<Long, Staff> staff = new ConcurrentHashMap<>();

    public Staff create(String name, String role) {
        long id = idSeq.incrementAndGet();
        Staff s = new Staff(id, name, role);
        staff.put(id, s);
        return s;
    }

    public Optional<Staff> findById(long id) {
        return Optional.ofNullable(staff.get(id));
    }

    public Staff require(long id) {
        return findById(id).orElseThrow(() -> new NoSuchElementException("Staff not found: " + id));
    }

    public List<Staff> findAllSortedById() {
        List<Staff> list = new ArrayList<>(staff.values());
        list.sort(Comparator.comparingLong(Staff::getId));
        return list;
    }

    public void delete(long id) {
        if (staff.remove(id) == null) {
            throw new NoSuchElementException("Staff not found: " + id);
        }
    }
}
