package uk.ac.kingston.ci7250.ims.repo;

import org.springframework.stereotype.Repository;
import uk.ac.kingston.ci7250.ims.domain.ImageRecord;

import java.util.ArrayList;
import java.util.Comparator;
import java.util.List;
import java.util.Map;
import java.util.NoSuchElementException;
import java.util.Optional;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.atomic.AtomicLong;

@Repository
public class ImageRepository {
    private final AtomicLong idSeq = new AtomicLong(4000);
    private final Map<Long, ImageRecord> images = new ConcurrentHashMap<>();

    public long nextId() {
        return idSeq.incrementAndGet();
    }

    public ImageRecord create(ImageRecord image) {
        images.put(image.getId(), image);
        return image;
    }

    public Optional<ImageRecord> findById(long id) {
        return Optional.ofNullable(images.get(id));
    }

    public ImageRecord require(long id) {
        return findById(id).orElseThrow(() -> new NoSuchElementException("Image not found: " + id));
    }

    public List<ImageRecord> findByPatientIdSorted(long patientId) {
        List<ImageRecord> list = new ArrayList<>();
        for (ImageRecord r : images.values()) {
            if (r.getPatientId() == patientId) {
                list.add(r);
            }
        }
        list.sort(Comparator.comparing(ImageRecord::getUploadedAt));
        return list;
    }

    public void delete(long id) {
        if (images.remove(id) == null) {
            throw new NoSuchElementException("Image not found: " + id);
        }
    }
}
