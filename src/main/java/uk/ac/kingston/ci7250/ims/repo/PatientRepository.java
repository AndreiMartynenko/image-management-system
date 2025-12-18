package uk.ac.kingston.ci7250.ims.repo;

import org.springframework.stereotype.Repository;
import uk.ac.kingston.ci7250.ims.domain.Patient;

import java.util.ArrayList;
import java.util.Comparator;
import java.util.List;
import java.util.Map;
import java.util.NoSuchElementException;
import java.util.Optional;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.atomic.AtomicLong;

@Repository
public class PatientRepository {
    private final AtomicLong idSeq = new AtomicLong(1000);
    private final Map<Long, Patient> patients = new ConcurrentHashMap<>();

    public Patient create(String name, String address, String conditions, String diagnosis) {
        long id = idSeq.incrementAndGet();
        Patient p = new Patient(id, name, address, conditions, diagnosis);
        patients.put(id, p);
        return p;
    }

    public Optional<Patient> findById(long id) {
        return Optional.ofNullable(patients.get(id));
    }

    public Patient require(long id) {
        return findById(id).orElseThrow(() -> new NoSuchElementException("Patient not found: " + id));
    }

    public List<Patient> findAllSortedById() {
        List<Patient> list = new ArrayList<>(patients.values());
        list.sort(Comparator.comparingLong(Patient::getId));
        return list;
    }

    public int count() {
        return patients.size();
    }

    public void delete(long id) {
        if (patients.remove(id) == null) {
            throw new NoSuchElementException("Patient not found: " + id);
        }
    }
}
